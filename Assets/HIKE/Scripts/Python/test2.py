import os
import numpy as np
import vtk
from alive_progress import alive_bar

# Verzeichnis angeben
directory = "data"

# Alle Dateien im Verzeichnis auflisten
files = [file for file in os.listdir(directory) if file.endswith(".xyz")]
coordinates = np.empty((0, 3))

# Extrahieren der X, Y und Z Koordinaten
points = vtk.vtkPoints()
length = 0
for file_name in files:
    file_path = os.path.join(directory, file_name)
    print("Verarbeite Datei:", file_name)
    
    num_lines = sum(1 for _ in open(file_path))
    # Datei öffnen und Zeilen einlesen
    with open(file_path, 'rb') as file:
        with alive_bar(num_lines) as inner_bar:
            for line in file:
                coord = [float(num) for num in line.split()]
                points.InsertNextPoint(coord)
                length = length + 1
                #coordinates = np.vstack([coordinates, coord])
                inner_bar()
        

# Erstellen von Polygonen für das Mesh
polygon = vtk.vtkPolygon()
polygon.GetPointIds().SetNumberOfIds(length)
for i in range(length):
    polygon.GetPointIds().SetId(i, i)

# Erstellen von Zellen für das Mesh
cells = vtk.vtkCellArray()
cells.InsertNextCell(polygon)

# Erstellen eines PolyData-Objekts und Zuweisen der Punkte und Zellen
polydata = vtk.vtkPolyData()
polydata.SetPoints(points)
polydata.SetPolys(cells)

# Schreiben des PolyData in eine VTK-Datei
writer = vtk.vtkPolyDataWriter()
writer.SetInputData(polydata)
writer.SetFileName("output_mesh.vtk")
writer.Write()