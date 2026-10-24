import zlib
import struct

def write_png(filename, width, height, rgb_buffer):
    # rgb_buffer is bytearray of length width * height * 3
    # PNG raw scanlines have 1 filter byte (0 = None) at the beginning of each row
    raw_scanlines = bytearray()
    row_bytes = width * 3
    for y in range(height):
        raw_scanlines.append(0) # filter type 0
        start = y * row_bytes
        raw_scanlines.extend(rgb_buffer[start:start + row_bytes])

    compressed = zlib.compress(bytes(raw_scanlines), level=6)

    def chunk(chunk_type, data):
        c = chunk_type + data
        crc = zlib.crc32(c) & 0xffffffff
        return struct.pack(">I", len(data)) + c + struct.pack(">I", crc)

    header = b'\x89PNG\r\n\x1a\n'
    ihdr = struct.pack(">IIBBBBB", width, height, 8, 2, 0, 0, 0) # 8-bit truecolor RGB
    idat = compressed

    with open(filename, 'wb') as f:
        f.write(header)
        f.write(chunk(b'IHDR', ihdr))
        f.write(chunk(b'IDAT', idat))
        f.write(chunk(b'IEND', b''))

# Test
w, h = 100, 100
buf = bytearray(w * h * 3)
for y in range(h):
    for x in range(w):
        idx = (y * w + x) * 3
        buf[idx] = int(x / w * 255)
        buf[idx+1] = int(y / h * 255)
        buf[idx+2] = 200

write_png('/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/test.png', w, h, buf)
print('PNG written successfully!')
