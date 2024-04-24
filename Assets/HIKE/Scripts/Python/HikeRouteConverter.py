from bs4 import BeautifulSoup
from alive_progress import alive_it
import os
import json
import lxml

input_folder = "D:/Windows/Desktop/data/routes/"
output_folder = "D:/Windows/Desktop/data/routes/"

files = [file for file in os.listdir(input_folder) if file.endswith(".gpx")]

for file_name in files:
    file_path = os.path.join(input_folder, file_name)
    print("Verarbeite Datei:", file_name)

    sights = []
    trackpoints = []

    with open(file_path, "r") as input:
        text = input.read()
        print(text)
        html = BeautifulSoup(text, "lxml-xml")
        trackpointHtmls = html.find_all("trkpt")

        for trkptHtml in trackpointHtmls:
            lat = float(trkptHtml["lat"])
            lon = float(trkptHtml["lon"])
            ele = float(trkptHtml.ele.string)
            trackpoint = {
                "lat": lat,
                "lon": lon,
                "ele": ele
            }
            trackpoints.append(trackpoint)

    route = {
        "name":html.find("name").string,
        "author":html.find("author").find("name").string,
        "trackpoints": trackpoints
    }

    out_file_path = os.path.join(output_folder, file_name + ".json")
    with open(out_file_path, "w") as out:
        out.write(json.dumps(route))
