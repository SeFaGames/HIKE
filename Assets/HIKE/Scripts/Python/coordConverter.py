import os
from alive_progress import alive_it

# Verzeichnis angeben
directory = "D:/Windows/Desktop/data/DGM5"
out_file = "D:/Windows/Desktop/data/harz_combined_out.xyz"

# Alle Dateien im Verzeichnis auflisten
files = [file for file in os.listdir(directory) if file.endswith(".xyz")]

# Extrahieren der X, Y und Z Koordinaten
length = 0
with open(out_file, "w") as out:
        for file_name in alive_it(files):
            file_path = os.path.join(directory, file_name)
            print("Verarbeite Datei:", file_name)
            
            name_segs = file_name.split('_')
            if len(name_segs) != 6 or name_segs[1] != "32" or int(name_segs[2]) < 600 or int(name_segs[2]) > 680 or int(name_segs[3]) < 5705 or int(name_segs[3]) > 5755:
                 continue

            # Datei öffnen und Zeilen einlesen
            with open(file_path, 'rb') as file:
                for line in file:
                    coord = [float(num) for num in line.split()]
                    # coord[0] = coord[0] - 608800
                    # coord[1] = coord[1] - 5736000
                    output_line = "%s %s %s\n" % (coord[0], coord[1], coord[2])
                    out.write(output_line)
