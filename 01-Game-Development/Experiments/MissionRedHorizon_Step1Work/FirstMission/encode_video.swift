// Encode a folder of numbered JPEG frames into an H.264 MP4 with AVFoundation (no ffmpeg needed).
// usage: swift encode_video.swift <frames dir> <out.mp4> [fps=30] [bitrate=12000000] [width=source] [first frame] [last frame]
import Foundation
import AVFoundation
import CoreGraphics
import ImageIO
import CoreVideo

let args = CommandLine.arguments
guard args.count >= 3 else { print("usage: encode_video.swift <frames> <out.mp4> [fps] [bitrate]"); exit(2) }
let dir = URL(fileURLWithPath: args[1]); let out = URL(fileURLWithPath: args[2])
let fps = Int32(args.count > 3 ? Int(args[3]) ?? 30 : 30)
let bitrate = args.count > 4 ? Int(args[4]) ?? 12_000_000 : 12_000_000
var files = try FileManager.default.contentsOfDirectory(atPath: dir.path).filter { $0.hasSuffix(".jpg") }.sorted()
let outW = args.count > 5 ? Int(args[5]) ?? 0 : 0
let firstF = args.count > 6 ? Int(args[6]) ?? 0 : 0
let lastF = args.count > 7 ? Int(args[7]) ?? files.count - 1 : files.count - 1
files = Array(files[max(0, firstF)...min(files.count - 1, lastF)])
guard let first = files.first, let src0 = CGImageSourceCreateWithURL(dir.appendingPathComponent(first) as CFURL, nil), let img0 = CGImageSourceCreateImageAtIndex(src0, 0, nil) else { print("no frames"); exit(1) }
let w = (outW > 0 ? outW : img0.width) & ~1, h = (outW > 0 ? outW * img0.height / img0.width : img0.height) & ~1
try? FileManager.default.removeItem(at: out)
let writer = try AVAssetWriter(outputURL: out, fileType: .mp4)
let input = AVAssetWriterInput(mediaType: .video, outputSettings: [
    AVVideoCodecKey: AVVideoCodecType.h264, AVVideoWidthKey: w, AVVideoHeightKey: h,
    AVVideoCompressionPropertiesKey: [AVVideoAverageBitRateKey: bitrate, AVVideoProfileLevelKey: AVVideoProfileLevelH264HighAutoLevel, AVVideoMaxKeyFrameIntervalKey: Int(fps) * 2]])
input.expectsMediaDataInRealTime = false
let adaptor = AVAssetWriterInputPixelBufferAdaptor(assetWriterInput: input, sourcePixelBufferAttributes: [
    kCVPixelBufferPixelFormatTypeKey as String: kCVPixelFormatType_32ARGB, kCVPixelBufferWidthKey as String: w, kCVPixelBufferHeightKey as String: h])
writer.add(input); writer.startWriting(); writer.startSession(atSourceTime: .zero)
let space = CGColorSpaceCreateDeviceRGB()
for (i, f) in files.enumerated() {
    autoreleasepool {
        guard let src = CGImageSourceCreateWithURL(dir.appendingPathComponent(f) as CFURL, nil), let img = CGImageSourceCreateImageAtIndex(src, 0, nil) else { return }
        while !input.isReadyForMoreMediaData { usleep(2000) }
        var pb: CVPixelBuffer?
        CVPixelBufferPoolCreatePixelBuffer(nil, adaptor.pixelBufferPool!, &pb)
        guard let buf = pb else { return }
        CVPixelBufferLockBaseAddress(buf, [])
        let ctx = CGContext(data: CVPixelBufferGetBaseAddress(buf), width: w, height: h, bitsPerComponent: 8, bytesPerRow: CVPixelBufferGetBytesPerRow(buf), space: space, bitmapInfo: CGImageAlphaInfo.noneSkipFirst.rawValue)!
        ctx.interpolationQuality = .high; ctx.draw(img, in: CGRect(x: 0, y: 0, width: w, height: h))
        CVPixelBufferUnlockBaseAddress(buf, [])
        adaptor.append(buf, withPresentationTime: CMTime(value: CMTimeValue(i), timescale: fps))
        if i % 600 == 0 { print("frame \(i)/\(files.count)") }
    }
}
input.markAsFinished()
let sem = DispatchSemaphore(value: 0)
writer.finishWriting { sem.signal() }
sem.wait()
print(writer.status == .completed ? "OK \(files.count) frames -> \(out.path)" : "FAILED \(String(describing: writer.error))")
