import os
import json
import struct
from alive_progress import alive_it, alive_bar

# Verzeichnis angeben
directory = "D:/Windows/Desktop/data/brocken_sample"
index_out_file = "D:/Windows/Desktop/data/brocken_dgm_index.json"
data_out_file = "D:/Windows/Desktop/data/brocken_dgm.json"
data_out_binary = "D:/Windows/Desktop/data/brocken_dgm.bytes"

index = {
    "x":{
        "min":200000000,
        "max":0,
        "diff":0
    },
    "y":{
        "min":200000000,
        "max":0,
        "diff":0
    },
    "z":{
        "min":200000000,
        "max":0,
        "diff":0
    },
    "heightmap":{
        "x":0,
        "z":0
    },
    "gitterweite":5,
    "dgm_sample_size":0
}

bounds = {
    "lowerLon":608, #Brocken = 608, Harz = 608
    "upperLon":614, #Brocken = 612, Harz = 680
    "lowerLat":5736, #Brocken = 5736, Harz = 5704/5706   
    "upperLat":5742  #Brocken = 5740, Harz = 5756
}

print(directory)

# Alle Dateien im Verzeichnis auflisten
files = [file for file in os.listdir(directory) 
            if  file.endswith(".xyz") 
                and len(file.split('_')) == 6 
                and int(file.split('_')[2]) >= bounds["lowerLon"] 
                and int(file.split('_')[2]) < bounds["upperLon"] 
                and int(file.split('_')[3]) >= bounds["lowerLat"] 
                and int(file.split('_')[3]) < bounds["upperLat"]]

print("File count: %s" % (len(files)))
print("Ascertaining DGM bounds: ")
for file_name in alive_it(files):
        file_path = os.path.join(directory, file_name)

        with open(file_path, 'r') as file:
            for line in file:
                index["dgm_sample_size"] +=1
                coord = [float(num) for num in line.split()]

#                 if coord[0] < index["x"]["min"]:
#                     index["x"]["min"] = coord[0]

#                 if coord[0] > index["x"]["max"]:
#                     index["x"]["max"] = coord[0]

#                 if coord[1] < index["z"]["min"]:
#                     index["z"]["min"] = coord[1]

#                 if coord[1] > index["z"]["max"]:
#                     index["z"]["max"] = coord[1]

                if coord[2] < index["y"]["min"]:
                    index["y"]["min"] = coord[2]

                if coord[2] > index["y"]["max"]:
                    index["y"]["max"] = coord[2]

index["x"]["min"] = bounds["lowerLon"]*1000
index["x"]["max"] = bounds["upperLon"]*1000
index["z"]["min"] = bounds["lowerLat"]*1000
index["z"]["max"] = bounds["upperLat"]*1000

print("Calculating coordinate diffrence and array size: ")
index["x"]["diff"] = index["x"]["max"] - index["x"]["min"]
index["y"]["diff"] = index["y"]["max"] - index["y"]["min"]
index["z"]["diff"] = index["z"]["max"] - index["z"]["min"]

index["heightmap"]["x"] = round(index["x"]["diff"] / index["gitterweite"]) + 1
index["heightmap"]["z"] = round(index["z"]["diff"] / index["gitterweite"]) + 1

print("Determinded array size to be x: %s and z: %s" % (index["heightmap"]["x"], index["heightmap"]["z"]))

print("Printing bounds to %s: " % (index_out_file))

with open(index_out_file, "w") as index_file:
    index_file.write(json.dumps(index))

print("Combining DGM into a single array: ")

heightmap = [[0.0] *index["heightmap"]["z"] for _ in range(index["heightmap"]["x"])]
for file_name in alive_it(files):
    file_path = os.path.join(directory, file_name)
    with open(file_path, 'r') as file:
        for line in file:
            coord = [float(num) for num in line.split()]
            x = int(coord[0])
            z = int(coord[1])
            y = coord[2]

            xIndex = round((x - index["x"]["min"]) / index["gitterweite"])
            zIndex = round((z - index["z"]["min"]) / index["gitterweite"])
            height = (y - index["y"]["min"]) / index["y"]["diff"]

            heightmap[xIndex][zIndex] = height

dgmHeightmap = {}
dgmHeightmap["heightmap"] = heightmap

print("Printing to json file: ")

with open(data_out_file, "w") as data_out:
    data_out.write(json.dumps(dgmHeightmap))

print("Printing to binary file: ")

data_binary = open(data_out_binary, "wb")
with alive_bar(index["heightmap"]["z"]*index["heightmap"]["x"]) as bar:
    for z in range(index["heightmap"]["z"]):
        for x in range(index["heightmap"]["x"]):
            value = heightmap[x][z]
            hbytes = struct.pack('<f', value)
            data_binary.write(hbytes)
            bar()
data_binary.close()

#
# Index File Format:
# =========================
# min, max: X, Y, Z
# diff: X, Y, Z
# array length: X, Z
# schrittweite
# Anz. Datenpunkte
# 
# Data File Format:
# 
#

# Calculate bounds
# Create index file



