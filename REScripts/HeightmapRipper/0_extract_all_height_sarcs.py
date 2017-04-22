import os
import glob
import re

minsmax = (0, 0)
sparseFileArray = {}

fileNames = glob.glob("../TESTDATA/height/*hght*")
for fileName in fileNames:
    os.system("mono ../BoTK.exe --input " + fileName + " --output ../TESTDATA/height_raw")
