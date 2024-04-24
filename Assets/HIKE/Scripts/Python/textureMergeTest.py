import os
import tifftools
import PIL
from PIL import Image

directory = "D:/Windows/Desktop/data/Orthofotos"
output_file = "D:/Windows/Desktop/data/Orthofotos/merged.tif"

# TRY 1
#files = [file for file in os.listdir(directory) 
#            if  file.endswith(".tif")]

#input1 = tifftools.read_tiff(os.path.join(directory, files[0]))
#input2 = tifftools.read_tiff(os.path.join(directory, files[1]))

#input1['ifds'].extend(input2['ifds'])
#tifftools.write_tiff(input1, output_file)

# TRY 2
#tiff_files_li=[]
#for ti in os.listdir(directory):
#    if ti.endswith('.tif'):
#        tiff_files_li.append(os.path.join(directory,ti))


def merge_tiles(tile_paths, output_path):
    # Öffne die vier Kachelbilder
    tiles = [Image.open(os.path.join(directory, path)) for path in tile_paths]

    # Bestimme die Breite und Höhe der Kacheln
    tile_width, tile_height = tiles[0].size

    # Bestimme die Gesamtbreite und -höhe des zusammengesetzten Bildes
    total_width = tile_width * 3  # Hier gehe ich davon aus, dass die Kacheln in einer 2x2-Matrix angeordnet sind
    total_height = tile_height * 3

    # Erstelle ein neues Bild mit der Gesamtgröße
    merged_image = Image.new('RGB', (total_width, total_height))

    # Füge die Kacheln in das zusammengesetzte Bild ein
    for i in range(3):  # Anzahl der Zeilen
        for j in range(3):  # Anzahl der Spalten
            merged_image.paste(tiles[i * 3 + j], (j * tile_width, i * tile_height))

    # Speichere das zusammengesetzte Bild
    merged_image.save(output_path)

def extract_tiles(input_image_path, tile_size, output_folder):
    # Öffne das große TIFF-Bild
    input_image = Image.open(input_image_path)

    # Bestimme die Breite und Höhe des großen Bildes
    image_width, image_height = input_image.size

    # Berechne die Anzahl der Kacheln in horizontaler und vertikaler Richtung
    num_tiles_x = image_width // tile_size[0]
    num_tiles_y = image_height // tile_size[1]

    # Schneide die Kacheln aus und speichere sie einzeln
    for y in range(num_tiles_y):
        for x in range(num_tiles_x):
            left = x * tile_size[0]
            lower = image_height-(y * tile_size[1])
            upper = lower - tile_size[1]
            right = left + tile_size[0]
            

            # Schneide die Kachel aus
            tile = input_image.crop((left, upper, right, lower))

            # Speichere die Kachel
            tile_path = f"{output_folder}/tile_{x}_{y}.tif"
            tile.save(tile_path)

# Beispielaufruf der Funktion mit Pfaden zu den Kachelbildern und dem Ausgabepfad für das zusammengesetzte Bild
tile_paths = [file for file in os.listdir(directory) 
                if  file.endswith(".tif")]

PIL.Image.MAX_IMAGE_PIXELS = 933120000

merge_tiles(tile_paths, output_file)
# Beispielaufruf der Funktion mit Pfaden und Kachelgröße
tile_size = (12830, 12830)  # Größe der Kacheln (Breite x Höhe)
output_folder = 'D:/Windows/Desktop/data/Orthofotos/tiles'  # Ordner zum Speichern der Kachelbilder
extract_tiles(output_file, tile_size, output_folder)
