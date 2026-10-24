// Edits recorded player frames into an H.264 MP4: title card, segment fades, captions, end card.
// usage: swift encode.swift <framesDir> <captions.txt> <out.mp4>
import AVFoundation
import AppKit
import CoreText

let args = CommandLine.arguments
let framesDir = URL(fileURLWithPath: args[1]), captionsPath = args[2], outURL = URL(fileURLWithPath: args[3])
let W = 1280, H = 720, fps: Int32 = 30, fade = 8

let files = try FileManager.default.contentsOfDirectory(atPath: framesDir.path).filter { $0.hasSuffix(".jpg") }.sorted()
var cuts: [(Int, String)] = try String(contentsOfFile: captionsPath, encoding: .utf8).split(separator: "\n").compactMap {
    let p = $0.split(separator: "|", maxSplits: 1); guard p.count == 2, let f = Int(p[0]) else { return nil }; return (f, String(p[1]))
}
cuts.sort { $0.0 < $1.0 }

try? FileManager.default.removeItem(at: outURL)
let writer = try AVAssetWriter(outputURL: outURL, fileType: .mp4)
let input = AVAssetWriterInput(mediaType: .video, outputSettings: [
    AVVideoCodecKey: AVVideoCodecType.h264, AVVideoWidthKey: W, AVVideoHeightKey: H,
    AVVideoCompressionPropertiesKey: [AVVideoAverageBitRateKey: 2_500_000, AVVideoProfileLevelKey: AVVideoProfileLevelH264HighAutoLevel]])
input.expectsMediaDataInRealTime = false
let adaptor = AVAssetWriterInputPixelBufferAdaptor(assetWriterInput: input, sourcePixelBufferAttributes: [
    kCVPixelBufferPixelFormatTypeKey as String: kCVPixelFormatType_32ARGB, kCVPixelBufferWidthKey as String: W, kCVPixelBufferHeightKey as String: H])
writer.add(input); writer.startWriting(); writer.startSession(atSourceTime: .zero)
var outFrame: Int64 = 0

func text(_ ctx: CGContext, _ s: String, _ size: CGFloat, _ x: CGFloat, _ y: CGFloat, bold: Bool = false, alpha: CGFloat = 1, center: Bool = false) {
    let font = NSFont.systemFont(ofSize: size, weight: bold ? .semibold : .regular)
    let attr = NSAttributedString(string: s, attributes: [.font: font, .foregroundColor: NSColor(white: 1, alpha: alpha)])
    let line = CTLineCreateWithAttributedString(attr)
    let w = CTLineGetTypographicBounds(line, nil, nil, nil)
    ctx.textPosition = CGPoint(x: center ? (CGFloat(W) - CGFloat(w)) / 2 : x, y: y)
    CTLineDraw(line, ctx)
}

func emit(_ draw: (CGContext) -> Void) {
    while !input.isReadyForMoreMediaData { Thread.sleep(forTimeInterval: 0.005) }
    var pb: CVPixelBuffer?; CVPixelBufferPoolCreatePixelBuffer(nil, adaptor.pixelBufferPool!, &pb)
    let buf = pb!; CVPixelBufferLockBaseAddress(buf, [])
    let ctx = CGContext(data: CVPixelBufferGetBaseAddress(buf), width: W, height: H, bitsPerComponent: 8, bytesPerRow: CVPixelBufferGetBytesPerRow(buf),
                        space: CGColorSpaceCreateDeviceRGB(), bitmapInfo: CGImageAlphaInfo.premultipliedFirst.rawValue)!
    ctx.setFillColor(CGColor(gray: 0, alpha: 1)); ctx.fill(CGRect(x: 0, y: 0, width: W, height: H))
    draw(ctx)
    CVPixelBufferUnlockBaseAddress(buf, [])
    adaptor.append(buf, withPresentationTime: CMTime(value: outFrame, timescale: fps)); outFrame += 1
}

func card(_ lines: [(String, CGFloat, Bool)], frames n: Int) {
    for i in 0..<n {
        let a = CGFloat(min(1, Double(min(i, n - 1 - i)) / Double(fade)))
        emit { ctx in
            var y = CGFloat(H) / 2 + CGFloat(lines.count - 1) * 22
            for (s, size, b) in lines { text(ctx, s, size, 0, y, bold: b, alpha: a, center: true); y -= size + 18 }
        }
    }
}

card([("AstroGateway: Mission Red Horizon", 46, true), ("Real gameplay capture, macOS build (Visual Upgrade v1)", 22, false)], frames: 75)

for (i, name) in files.enumerated() {
    guard let img = NSImage(contentsOf: framesDir.appendingPathComponent(name))?.cgImage(forProposedRect: nil, context: nil, hints: nil) else { continue }
    let seg = cuts.lastIndex { $0.0 <= i } ?? 0
    let start = cuts.isEmpty ? 0 : cuts[seg].0
    let end = seg + 1 < cuts.count ? cuts[seg + 1].0 : files.count
    let fadeA = min(1, Double(min(i - start, end - 1 - i)) / Double(fade))
    emit { ctx in
        // aspect-fill into 1280x720
        let s = max(CGFloat(W) / CGFloat(img.width), CGFloat(H) / CGFloat(img.height))
        let dw = CGFloat(img.width) * s, dh = CGFloat(img.height) * s
        ctx.interpolationQuality = .high
        ctx.setAlpha(CGFloat(fadeA))
        ctx.draw(img, in: CGRect(x: (CGFloat(W) - dw) / 2, y: (CGFloat(H) - dh) / 2, width: dw, height: dh))
        ctx.setAlpha(1)
        if !cuts.isEmpty && i - start < 75 {
            let ca = CGFloat(min(1, Double(min(i - start, 74 - (i - start))) / 10.0)) * CGFloat(fadeA)
            let label = cuts[seg].1
            ctx.setFillColor(CGColor(gray: 0, alpha: 0.55 * ca))
            let lw = CTLineGetTypographicBounds(CTLineCreateWithAttributedString(NSAttributedString(string: label, attributes: [.font: NSFont.systemFont(ofSize: 20, weight: .semibold)])), nil, nil, nil)
            ctx.fill(CGRect(x: 24, y: 70, width: CGFloat(lw) + 32, height: 40))
            text(ctx, label, 20, 40, 83, bold: true, alpha: ca)
        }
    }
}

card([("AstroGateway", 40, true), ("Captured from the running game. Frames unaltered apart from fades and captions.", 18, false)], frames: 60)

input.markAsFinished()
let done = DispatchSemaphore(value: 0)
writer.finishWriting { done.signal() }
done.wait()
print("status=\(writer.status.rawValue) frames=\(outFrame) seconds=\(Double(outFrame) / Double(fps)) error=\(String(describing: writer.error))")
