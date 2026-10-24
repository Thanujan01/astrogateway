import Foundation
import AVFoundation
import AppKit
let root = "/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp"
let asset = AVURLAsset(url: URL(fileURLWithPath: root + "/outputs/Stage08C/Android_Runtime_Preview.mp4"))
let generator = AVAssetImageGenerator(asset: asset)
generator.appliesPreferredTrackTransform = true
for seconds in [0.0,5.0,9.0] {
 let cg = try generator.copyCGImage(at: CMTime(seconds: seconds, preferredTimescale: 600), actualTime: nil)
 let data = NSBitmapImageRep(cgImage: cg).representation(using: .png, properties: [:])!
 try data.write(to: URL(fileURLWithPath: root + "/outputs/Stage08C1/Screenshots/Stage08C_Video_Frame_\(Int(seconds)).png"))
 print("Decoded actual frame at \(seconds)s: \(cg.width)x\(cg.height)")
}
