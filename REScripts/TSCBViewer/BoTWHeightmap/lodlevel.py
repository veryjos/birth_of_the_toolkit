class LodLevel:
    def __init__(self, lodlevel):
        self.lodlevel = lodlevel

        self.tiles = []


    def SetTile(self, tile):
        self.tiles.append(tile)
