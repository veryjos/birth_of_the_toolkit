import os
import glob
import re
import sys

heightmapDir = sys.argv[1]
outputDir = sys.argv[2]

fileNames = filter(lambda v: 'water' in v, os.listdir(heightmapDir))

for fileName in fileNames:
    print(fileName)
    os.system("mono ../../../BoTK/bin/Debug/BoTK.exe --input '" + heightmapDir + "/" + fileName + "' --output " + outputDir + " &")
