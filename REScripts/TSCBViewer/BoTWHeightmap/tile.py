class Tile:
    def __init__(self, unk1Instance):
        self.unk1Instance = unk1Instance

    def Draw(self, camera):
        [translation, zoom] = camera.GetTransform()

