from PIL import Image
import sys
import struct

f = open(sys.argv[1], 'rb')

imw = 64
imh = 64

im = Image.new('RGBA', (imw, imh))
pix = im.load()

def plotpix(x, y, c):
    pix[x, y] = c

i=0
p=0

while i < 32768:
    r = struct.unpack('>H', f.read(2))[0] / 65535.0
    r *= 255

    g = struct.unpack('>H', f.read(2))[0] / 65535.0
    g *= 255

    b = struct.unpack('>H', f.read(2))[0] / 65535.0
    b *= 255

    a = struct.unpack('>H', f.read(2))[0] / 65535.0
    a *= 255
    print a

    plotpix(p % imw, p / imw, (int(r), int(g), int(b), int(a)))

    p += 1
    i += 8

if len(sys.argv) == 3:
    im.save(sys.argv[2])
else:
    im.show()
