class Camera:
    epsilon = 1

    def __init__(self, bounds):
        self.lodLevel = 0
        self.pos = [0, 0]
        self.zoom = 5

        self.bounds = bounds

    def SetLodLevel(self, lodLevel):
        self.lodLevel = lodLevel

    def ZoomDelta(self, offset):
        self.zoom += offset * self.zoom

    def Translate(self, offset):
        self.pos[0] += offset[0] * (1.0 / self.zoom)
        self.pos[1] += offset[1] * (1.0 / self.zoom)

    def ScreenToWorldPoint(self, point):
        detransformed = (
                self.pos[0] + point[0],
                self.pos[1] + point[1]
            )

        descaled = (
                detransformed[0] / self.zoom,
                detransformed[1] / self.zoom
            )

        return (
                descaled[0] - self.bounds[0] / 2,
                descaled[1] - self.bounds[1] / 2
            )

    def TransformPoint(self, point):
        transformed = (
                point[0] - self.pos[0],
                point[1] - self.pos[1]
            )

        scaled = (
                transformed[0] * self.zoom,
                transformed[1] * self.zoom
            )

        return (
                scaled[0] + self.bounds[0] / 2,
                scaled[1] + self.bounds[1] / 2
            )

    def TransformRect(self, rect):
        tl = self.TransformPoint((rect[0], rect[1]))
        br = self.TransformPoint((rect[0] + rect[2], rect[1] + rect[3]))

        if tl[0] > self.bounds[0] or tl[1] > self.bounds[1]:
            return False

        if br[0] < 0 or br[1] < 0:
            return False

        return [
                tl[0] - self.epsilon, tl[1] - self.epsilon,
                br[0] - tl[0] + self.epsilon, br[1] - tl[1] + self.epsilon
            ]
