# MOP Builder
# This script let's you quickly compress new release to .zip file.
# Script version: 1.0.0 (14.12.2019)
#
# This file is distributed under the same license as the MOP is.

import os
import sys
import zipfile
from zipfile import ZipFile
from array import array
import shutil
print("=== Building the release... ===\n")

BASE_DIR = os.getcwd()


def make_zip(files, zipName):
    print('Creating new zip: {0}'.format(zipName))
    NEW_ZIP = ZipFile(BASE_DIR + "\\" + zipName, 'w', zipfile.ZIP_DEFLATED)

    for file in files:
        NEW_ZIP.write(file)

    NEW_ZIP.close()


os.chdir(BASE_DIR)
shutil.rmtree(BASE_DIR + "\\build", True)

os.mkdir("build")
shutil.copyfile("ActualMop\\bin\\Release\\ActualMop.dll",
                "build\\ActualMop.dll")
shutil.copyfile("PleaseReadMe.txt", "build\\PleaseReadMe.txt")
os.makedirs("build\\Assets\\ActualMop")
shutil.copyfile("MopAsset\\AssetBundles\\mop.unity3d",
                "build\\Assets\\ActualMop\\mop.unity3d")
os.chdir("build")

FILES = []
FILES.extend(["ActualMop.dll"])
FILES.extend(["PleaseReadme.txt"])
FILES.extend(["Assets\\ActualMop\\mop.unity3d"])
make_zip(FILES, "ActualMop.zip")

print("Done!\nQuitting...")
quit()
