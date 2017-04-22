import os
import glob
import re
import sys
import struct
import numpy as np

# heightmaps are named in this format:
# 5<A>0000<BBBB>.hght
# A = LOD level in hex
# B = index of image in hex
# 
# Total width of LOD level is equal to 2 ^ LOD level
# Square map
#
# So, when LOD level is 0, total heightmap size is 1x1, with 1 tile
# When LOD level is 1, total heightmap size is 2x2, with 4 tiles
# When LOD level is 2, total heightmap size is 4x4, with 16 tiles
#   In this case (note the 2x2 grouping, each SARC holds 4 tiles for each group):
#   5200000000 5200000001 5200000004 5200000005
#   5200000002 5200000003 5200000006 5200000007
#   5200000008 5200000009 520000000C 520000000D
#   520000000A 520000000B 520000000E 520000000F
#
# Tiles are always 256x256 pixels
# Each pixel is stored as big endian unsigned shorts (2bpp)
#
# Tiles are in groups of four
#
# If a heightmap isn't available, that means the LOD doesn't exist.
# You have to source the data from a lower LOD

lodLevel = int(sys.argv[1])
heightmapDir = sys.argv[2]
outFile = sys.argv[3]

edgeLength = 2 ** lodLevel
mapWidthTiles = edgeLength
mapHeightTiles = edgeLength

print(edgeLength)

fileNamePattern = heightmapDir + "/5%(lodLevel)d0000%(index)s.hght"

tileWidth = 256
tileHeight = 256

outWidth = mapWidthTiles * tileWidth
outHeight = mapHeightTiles * tileHeight

outPixels = [0] * outWidth * outHeight

for y in range(0, mapWidthTiles):
    sy = y * tileHeight

    for x in range(0, mapHeightTiles):
        sx = x * tileWidth

        def genHex(i, count):
            while i:
                yield ('%(num)02X' % {"num": i % 255})
                i >>= 8
                count += 1

            while count < 2:
                yield '00'
                count += 1

        # don't forget the 2x2 grouping :)
        majorGridX = x / 2
        majorGridY = y / 2

        indexNum =  (majorGridX * 4) + x % 2
        indexNum += ((y % 2) * 2) + majorGridY * 2 * mapWidthTiles

        indexArr = list(genHex(indexNum, 0))
        indexArr.reverse()
        indexStr = "".join(indexArr)

        fileName = fileNamePattern % {"lodLevel": lodLevel, "index": indexStr}

        print(fileName)
        try:
            f = open(fileName, 'rb')

            for py in range(0, tileHeight):
                for px in range(0, tileWidth):
                    h = struct.unpack("<H", f.read(2))[0]
                    pv = int((h / 65535.0) * 255.0)

                    outPixels[(sy + py) * outWidth + (sx + px)] = pv

            f.close()

        except:
            print("missing lodlevel " + str(lodLevel) + " for " + indexStr)

    sys.stdout.write('\n')

o = open(outFile, 'wb')

o.write(bytearray(outPixels))

o.close()
