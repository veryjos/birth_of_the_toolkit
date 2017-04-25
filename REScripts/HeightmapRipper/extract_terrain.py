import os
import glob
import re
import sys

terrainDir = sys.argv[1]
outputDir = sys.argv[2]

fileNames = filter(lambda v: 'hght' in v, os.listdir(terrainDir))
for fileName in fileNames:
    print(fileName)
    os.system("mono ../../BoTK/bin/Debug/BoTK.exe --input '" + terrainDir + "/" + fileName + "' --output " + outputDir + " &")

fileNames = filter(lambda v: 'mate' in v, os.listdir(terrainDir))
for fileName in fileNames:
    print(fileName)
    os.system("mono ../../BoTK/bin/Debug/BoTK.exe --input '" + terrainDir + "/" + fileName + "' --output " + outputDir + " &")

fileNames = filter(lambda v: 'water' in v, os.listdir(terrainDir))
for fileName in fileNames:
    print(fileName)
    os.system("mono ../../BoTK/bin/Debug/BoTK.exe --input '" + terrainDir + "/" + fileName + "' --output " + outputDir + " &")

fileNames = filter(lambda v: 'grass' in v, os.listdir(terrainDir))
for fileName in fileNames:
    print(fileName)
    os.system("mono ../../BoTK/bin/Debug/BoTK.exe --input '" + terrainDir + "/" + fileName + "' --output " + outputDir + " &")

