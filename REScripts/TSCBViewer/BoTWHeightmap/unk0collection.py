class Unk0Instance:
    def __init__(self, breader):
        self.breader = breader

        self.Parse()

    def Parse(self):
        self.index = self.breader.Read("I", 4)

        self.unk0 = self.breader.Read("f", 4)
        self.unk1 = self.breader.Read("f", 4)
        self.unk2 = self.breader.Read("f", 4)
        self.unk3 = self.breader.Read("f", 4)

class Unk0Collection:
    def __init__(self, heightmap, breader):
        self.heightmap = heightmap
        self.breader = breader

        self.instances = []

        self.Parse()

    def Parse(self):
        unk0 = self.breader.Read("I", 4)
        unk1 = self.breader.Read("I", 4)

        unk2 = self.breader.Read("f", 4)
        unk3 = self.breader.Read("I", 4)

        self.unk1CollectionStart = self.breader.Tell()
        self.size = self.breader.Read("I", 4)
        self.unk1CollectionStart += self.size

        # This is a list of offsets to Unk0Instances 
        for i in range(0, self.heightmap.unk0CollectionCount):
            offsetBase = self.breader.Tell()
            offset = self.breader.Read("I", 4)

            # Seek to the offset and create a new Unk0Instance
            self.breader.Seek(offsetBase + offset)
            instance = Unk0Instance(self.breader)
            
            self.instances.append(instance)

            # Return the reader to the offset table and skip the offset we
            # just read
            self.breader.Seek(offsetBase + 4)
