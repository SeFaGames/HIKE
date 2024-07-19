import re
import os
from pyproj import Transformer,CRS
from alive_progress import alive_it

jsonFiles = "D:/Windows/Desktop/xR/data/Wanderrouten"
outputDir = "D:/Windows/Desktop/xR/data/Wanderrouten/fixed"

testJson = "[[10.60907,51.726642,561.1],[10.608996,51.726686,561.8],[10.60889,51.726811,563.6]]"
regexString = r"\[(\d+\.\d+),(\d+\.\d+),(\d+\.\d+)\]"
pattern = re.compile(regexString, re.S)

crs_gps = CRS.from_epsg(4326)
crs_etrs = CRS.from_epsg(25832)

transformer = Transformer.from_crs(crs_gps, crs_etrs, always_xy=True)

def getReplacement(match):
    lon = float(match.group(1))
    lat = float(match.group(2))
    y = float(match.group(3))
    #print("Lat %s" % (lat))
    #print("Lon %s" % (lon))
    x, z = transformer.transform(lon, lat)
    #print("X %s" % (x))
    #print("Z %s" % (z))
    return "{\"x\":%s,\"z\":%s,\"y\":%s}" % (x, z, y)

files = [file for file in os.listdir(jsonFiles) if file.endswith(".json")]

for file_name in alive_it(files):
    file_path = os.path.join(jsonFiles, file_name)
    print("Verarbeite Datei:", file_name)
    with open(file_path, "r", encoding="utf8") as file:
        content = file.read()
        output = pattern.sub(lambda match: match.group(0).replace(match.group(0),getReplacement(match)), content)
        #match = re.match(regexString, file.read())
        # for match in re.finditer(pattern, testJson):
        #     if match:
        #         x,z,y = match.groups()
        #         print("{\"x\":%s,\"z\":%s,\"y\":%s}" % (x, z, y))
        #     else:
        #         print("no match")
        outputFile = os.path.join(outputDir, file_name)
        with open(outputFile, "w", encoding="utf8") as out:
            out.write(output)