import struct

class BinaryReader:
    def __init__(self, f, endian):
        self.endian = endian
        self.f = f

    def Tell(self):
        return self.f.tell()

    def Seek(self, offset):
        self.f.seek(offset)

    def Skip(self, offset):
        self.f.read(offset)

    def Peek(self, frmt, size):
        oldPos = self.Tell()
        result = self.Read(self.endian + frmt, size)
        self.Seek(oldPos)

        return result

    def PeekString(self):
        oldPos = self.Tell()
        result = self.ReadString()
        self.Seek(oldPos)

        return result

    def ReadString(self):
        chars = []

        while True:
            c = self.f.read(1) 
            if c == chr(0):
                return "".join(chars)
            else:
                chars.append(c)


    def Read(self, frmt, size):
        result = struct.unpack(self.endian + frmt, self.f.read(size))

        if len(result) == 1:
            return result[0]
        else:
            return result
