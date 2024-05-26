from pyproj import Transformer,CRS
from alive_progress import alive_it
import json
import numpy as np

crs_gps = CRS.from_epsg(4326)
crs_etrs = CRS.from_epsg(25832)

input_filename = "D:\Windows\Desktop\data\stempelstellenHWN_2024-04-04.txt"
output_filename = "D:\Windows\Desktop\data\stempelstellen_json.txt"

# Initialize the transformer to convert from WGS84 (EPSG:4326) to ETRS89 (EPSG:4258)
transformer = Transformer.from_crs(crs_gps, crs_etrs, always_xy=True)

# Example GPS coordinates in WGS84 to be converted
# Longitude (Längengrade), Latitude (Breitengrade) of Berlin, for instance
# longitude_wgs84, latitude_wgs84 = 51.841640, 10.579972

num_lines = sum(1 for _ in open(input_filename))
stempelstellen = []

with open(input_filename, 'r') as file:
    for line in file:
        data = {}
        elements = line.split(', ')
        print(elements[0])
        print(elements[3])
        
        coords = elements[3].split(' - ')
        lat = float(coords[0])
        lon = float(coords[1])

        #Convert Coordinate Reference System
        x, z = transformer.transform(lon, lat)

        data["id"] = int(elements[0])
        data["name"] = elements[1]
        data["y"] = float(elements[2])
        data["x"] = x
        data["z"] = z
        data["lat"] = lat
        data["lon"] = lon

        stempelstellen.append(data)

# Wrap the Array of Stempelstellen in a seperate object, because Unity can only deserialize "simple" data types directly and not arrays or lists
arrayObject = {}
arrayObject["stempelstellen"] = stempelstellen

with open(output_filename, 'w') as out:
    out.write(json.dumps(arrayObject))