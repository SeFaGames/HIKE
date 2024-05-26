import numpy as np
from alive_progress import alive_it, alive_bar

# Verzeichnis angeben
input_file = "D:/Windows/Desktop/data/harz_combined_out.xyz"
output_file = "D:/Windows/Desktop/data/harz_heightmap"
spacing = 5
sampleResolution = 14001
resolution = 4097

heightmap = [[0.0] *sampleResolution for _ in range(sampleResolution)]


minX = 607095
maxX = 676000
minZ = 5700000
maxZ = 5770000
minY = 58.9
maxY = 1141.07

# # Datei öffnen und Zeilen einlesen
# with open(input_file, 'rb') as file:
#         with alive_bar(num_lines) as inner_bar:
#             for line in file:
#                 coord = [float(num) for num in line.split()]

#                 if coord[0] < minX:
#                     minX = coord[0]

#                 if coord[0] > maxX:
#                     maxX = coord[0]

#                 if coord[1] < minZ:
#                     minZ = coord[1]

#                 if coord[1] > maxZ:
#                     maxZ = coord[1]

#                 if coord[2] < minY:
#                     minY = coord[2]

#                 if coord[2] > maxY:
#                     maxY = coord[2]

#                 inner_bar()


diffX = maxX - minX
diffY = maxY - minY
diffZ = maxZ - minZ

with open(input_file, 'rb') as file:
        for line in alive_it(file):
            coord = [float(num) for num in line.split()]
            x = int((coord[0] - minX)/spacing)
            z = int((coord[1] - minZ)/spacing)
            # Maximalhöhe = 1100 Minimalhöhe= 600 Differenz = 500
            y = (coord[2] - minY)/diffY

            heightmap[x][z] = y

with alive_bar(len(heightmap)*len(heightmap)) as bar:
    for l in range(int(sampleResolution/resolution) +1):
        for c in range(int(sampleResolution/resolution) + 1):
            out = open(output_file + "_" + str(l) + "_" + str(c) + ".raw", 'wb')
            i = 0
            while i-(l*(resolution-1)) < resolution:
                j = 0
                while j-(c*(resolution-1)) < resolution:
                    if i >= sampleResolution or j >= sampleResolution:
                        height_bytes = bytes(0)
                    else:
                        entry = heightmap[i][j]
                        height_bytes = int(entry * 65535).to_bytes(2, byteorder='little')
                    out.write(height_bytes)
                    j += 1
                i += 1
            out.close()

    # for height in heightmap:
    #     for entry in height:
    #         height_bytes = int(entry * 65535).to_bytes(2, byteorder='little')
    #         out.write(height_bytes)
    #         i += 1

    #         if i >= resolution*resolution:
    #             out.close()
    #             c += 1
    #             i = 0
    #             out = out = open(output_file + "_" + str(c) + ".raw", 'wb')
    #         bar()


        #         height_bytes = struct.pack('>H', int(entry * 65535))
        #         out.write(height_bytes)
        #     bar()