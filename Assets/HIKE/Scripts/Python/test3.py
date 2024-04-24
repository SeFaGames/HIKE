import vtk
import os
from alive_progress import alive_bar

# Verzeichnis angeben
directory = "data"

# Alle Dateien im Verzeichnis auflisten
files = [file for file in os.listdir(directory) if file.endswith(".xyz")]

# Extrahieren der X, Y und Z Koordinaten
points = vtk.vtkPoints()
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
                inner_bar()
            
nPoints = points.GetNumberOfPoints()
pointIds = vtk.vtkIdList()
pointIds.SetNumberOfIds(nPoints)
for i in range(nPoints):
    pointIds.SetId(i, i)

polyPoint = vtk.vtkCellArray()
polyPoint.InsertNextCell(pointIds)

polyData = vtk.vtkPolyData()
polyData.SetPoints(points)
polyData.SetVerts(polyPoint)

delaunay = vtk.vtkDelaunay2D()
delaunay.SetInputData(polyData)
delaunay.Update()
triangulatedPolyData = delaunay.GetOutput()

# Schreiben des PolyData in eine VTK-Datei
writer = vtk.vtkPolyDataWriter()
writer.SetInputData(triangulatedPolyData)
writer.SetFileName("output_mesh.vtk")
writer.Write()
