class StringTable:

    def __init__(self, breader):
        self.breader = breader

        self.strings = []
        self.stringsByOffset = {}

        self.Parse()

    def Parse(self):
        unk0 = self.breader.Read("I", 4)
        unk1 = self.breader.Read("I", 4)
        unk2 = self.breader.Read("I", 4)
        unk3 = self.breader.Read("I", 4)

        # Begin parsing out each string
        try:
            while True:
                # Strings a possibly null-padded, so skip any null bytes
                while True:
                    b = self.breader.Peek("B", 1)
    
                    if b == 0:
                        self.breader.Skip(1)
                        continue
                    else:
                        break

                stringValue = self.breader.ReadString()
                self.strings.append(stringValue)

        except:
            print "Reached EoF for string table generation"
    
    def GetString(self, index):
        return self.strings[index]

    def GetStringByOffset(self, offset):
        return self.stringsByOffset[offset]
