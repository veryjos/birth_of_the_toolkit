from .lodlevel import LodLevel
from .stringtable import StringTable

from .unk0collection import Unk0Collection
from .unk1collection import Unk1Collection

class Heightmap:
    def __init__(self, breader):
        self.breader = breader

        self.lodlevels = []
        self.stringTable = None

        self.Parse()

        print("created new heightmap with", breader)

    def Parse(self):
        magic = self.breader.Read("BBBB", 4)

        unk0 = self.breader.Read("I", 4)
        unk1 = self.breader.Read("I", 4)
        self.strTableOffset = self.breader.Read("I", 4)

        unk2 = self.breader.Read("f", 4)
        unk3 = self.breader.Read("f", 4)

        self.unk0CollectionCount = self.breader.Read("I", 4)
        self.unk1CollectionCount = self.breader.Read("I", 4)

        restorePosition = self.breader.Tell()

        self.breader.Seek(self.strTableOffset)
        self.stringTable = StringTable(self.breader)

        self.breader.Seek(restorePosition)

        self.unk0Collection = Unk0Collection(self, self.breader)

        self.breader.Seek(self.unk0Collection.unk1CollectionStart)
        self.unk1Collection = Unk1Collection(self, self.breader)

    def Draw(self, screen, camera):
        self.unk1Collection.Draw(screen, camera)

    def SetTile(self, x, y, tile):
        self.tiles[x][y] = tile

    def GetTile(self, x, y):
        return self.tiles[x][y]


