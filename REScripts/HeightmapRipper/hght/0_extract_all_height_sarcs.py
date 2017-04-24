import os
import glob
import re
import sys

heightmapDir = sys.argv[1]
outputDir = sys.argv[2]

fileNames = glob.glob(heightmapDir + "/*hght*")

for fileName in fileNames:
    os.system("mono ../BoTK.exe --input " + fileName + " --output " + outputDir)
