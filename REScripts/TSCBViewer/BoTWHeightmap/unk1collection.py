import random
import sys
import struct
from pprint import pprint

import pygame

from .config import Config

class Unk1Instance:
    coverageColors = {
        32.0:   255 - 8 * (255 / 8),
        16.0:   255 - 7 * (255 / 8),
        8.0:    255 - 6 * (255 / 8),
        4.0:    255 - 5 * (255 / 8),
        2.0:    255 - 4 * (255 / 8),
        1.0:    255 - 3 * (255 / 8),
        0.5:    255 - 2 * (255 / 8),
        0.25:   255 - 1 * (255 / 8),
        0.125:  255 - 0 * (255 / 8)
    }

    def PrintDebug(self):
        print '\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nDebug:'
        print "centerX:", self.centerX
        print "centerY:", self.centerY
        print "edgeLength:", self.edgeLength

        print ''

        print "unk0:", self.unk0
        print "unk1:", self.unk1
        print "unk2:", self.unk2
        print "unk3:", self.unk3
        print "unk4:", self.unk4

        print ''

        print "strOffset:", self.strOffset
        print "fileName:", self.fileName

        print "unk5:", self.unk5
        print "unk6:", self.unk6
        print "unk7:", self.unk7

        print ''

        print "variableCount:", self.variableCount
        print self.variables

    def __init__(self, breader):
        self.breader = breader
        self.color = (
                int(64 + random.random() * (255 - 64)),
                int(64 + random.random() * (255 - 64)),
                int(64 + random.random() * (255 - 64))
            )

        self.variables = []
        self.texImage = None
        self.texBuffer = []

        self.Parse()

    def GetLodColor(self):
        c = self.coverageColors[self.edgeLength]

        return (c, 0, 255 - c)

    def CheckClick(self, camera, clickCoords):
        if camera.lodLevel != 'ALL' and self.edgeLength != camera.lodLevel:
            return
    
        transformed = camera.TransformRect([
            self.centerX - self.edgeLength / 2, self.centerY - self.edgeLength / 2,
            self.edgeLength, self.edgeLength
        ])

        if transformed:
            if clickCoords[0] > transformed[0] and clickCoords[1] > transformed[1] and clickCoords[0] < transformed[0] + transformed[2] and clickCoords[1] < transformed[1] + transformed[3]:
                self.PrintDebug()
    
    def Draw(self, screen, camera):
        if camera.lodLevel != 'ALL' and self.edgeLength != camera.lodLevel:
            self.LazyUnloadTexture()

            return

        transformed = camera.TransformRect([
            self.centerX - self.edgeLength / 2, self.centerY - self.edgeLength / 2,
            self.edgeLength, self.edgeLength
        ])

        if transformed:
            if Config.draw_grid:
                transformed[0] += 2
                transformed[1] += 2
                transformed[2] -= 2
                transformed[3] -= 2

            if Config.draw_textures:
                texImage = pygame.transform.scale(self.LazyLoadTexture(), (int(transformed[2]), int(transformed[3])))
                screen.blit(texImage, (transformed[0], transformed[1]))
            else:
                self.LazyUnloadTexture()

                if Config.draw_coverage:
                    pygame.draw.rect(screen, self.GetLodColor(), transformed)
                else:
                    pygame.draw.rect(screen, self.color, transformed)
        else:
            self.LazyUnloadTexture()

    def LazyLoadTexture(self):
        if self.texImage:
            return self.texImage

        f = open(sys.argv[2] + '/' + self.fileName + '.hght', 'rb')

        heights = struct.unpack("<65536H", f.read(256*256*2))
        self.texBuffer = bytearray(256*256*3)
        j = 0
        for i in range(0, 256*256):
            pixelValue = int((heights[i] / 65535.0) * 255.0)

            self.texBuffer[j + 0] = pixelValue
            # self.texBuffer[j + 1] = pixelValue
            # self.texBuffer[j + 2] = pixelValue

            j += 3

        self.texImage = pygame.image.frombuffer(self.texBuffer, (256, 256), 'RGB')

        f.close()

        return self.texImage

    def LazyUnloadTexture(self):
        if self.texImage:
            self.texImage = None
            self.texBuffer = None

    def Parse(self):
        self.centerX = self.breader.Read("f", 4)
        self.centerY = self.breader.Read("f", 4)
        self.edgeLength = self.breader.Read("f", 4)

        self.unk0 = self.breader.Read("f", 4)
        self.unk1 = self.breader.Read("f", 4)
        self.unk2 = self.breader.Read("f", 4)
        self.unk3 = self.breader.Read("f", 4)

        self.unk4 = self.breader.Read("I", 4)

        self.strOffset = self.breader.Read("I", 4)

        oldPos = self.breader.Tell()
        self.breader.Seek(oldPos + self.strOffset - 4)

        self.fileName = self.breader.ReadString()

        self.breader.Seek(oldPos)

        self.unk5 = self.breader.Read("I", 4)
        self.unk6 = self.breader.Read("I", 4)
        self.unk7 = self.breader.Read("I", 4)

        if self.unk4 != 0:
            self.variableCount = self.breader.Read("I", 4)
            for i in range(0, self.variableCount):
                val = self.breader.Read("I", 4)
                self.variables.append(val)
        else:
            self.variableCount = 0
            self.variables = []

class Unk1Collection:
    def __init__(self, heightmap, breader):
        self.heightmap = heightmap
        self.breader = breader

        self.instances = []

        self.Parse()

    def Parse(self):
        # This is a list of offsets to Unk1Instances 
        for i in range(0, self.heightmap.unk1CollectionCount):
            restorePos = self.breader.Tell()
            offset = self.breader.Read("I", 4)

            # Seek to the offset and create a new Unk0Instance
            self.breader.Seek(restorePos + offset)
            instance = Unk1Instance(self.breader)
            
            self.instances.append(instance)

            # Return the reader to the offset table and skip the offset we
            # just read
            self.breader.Seek(restorePos + 4)

    def Draw(self, screen, camera):
        for unk1 in self.instances:
            unk1.Draw(screen, camera)

    def CheckClick(self, camera, clickCoords):
        for unk1 in self.instances:
            unk1.CheckClick(camera, clickCoords)
