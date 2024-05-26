import os
import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from alive_progress import alive_bar

# Verzeichnis angeben
directory = "data"

# Alle Dateien im Verzeichnis auflisten
files = [file for file in os.listdir(directory) if file.endswith(".xyz")]

# NumPy-Array initialisieren
coordinates = np.empty((0, 3))

# Für jede Datei im Verzeichnis
for file_name in files:
    file_path = os.path.join(directory, file_name)
    print("Verarbeite Datei:", file_name)

    num_lines = sum(1 for _ in open(file_path))

    # Datei öffnen und Zeilen einlesen
    with open(file_path, 'r') as file:
        with alive_bar(num_lines) as bar:
            for line in file:
                # Aufteilen der Zeile in Zahlen (Trennzeichen anpassen, falls nötig)
                numbers = [float(num) for num in line.split()]
                
                # Hinzufügen der Zahlen als neue Zeile zum NumPy-Array
                coordinates = np.vstack([coordinates, numbers])
                bar()

# Extrahieren der X, Y und Z Koordinaten
x = coordinates[:, 0]
y = coordinates[:, 1]
z = coordinates[:, 2]

# Erzeugen eines 3D-Plots
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')

# Erzeugen des Meshs
ax.plot_trisurf(x, y, z, cmap='viridis', edgecolor='none')

# Beschriftung der Achsen
ax.set_xlabel('X')
ax.set_ylabel('Y')
ax.set_zlabel('Z')

# Anzeigen des Plots
plt.show()
