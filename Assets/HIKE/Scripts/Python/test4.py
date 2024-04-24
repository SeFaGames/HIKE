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
polyData = vtk.vtkPolyData()
polyData.SetPoints(points)

# Gaussian kernel
gaussian_kernel = vtk.vtkGaussianKernel()
gaussian_kernel.SetSharpness(2)
gaussian_kernel.SetRadius(12)

interpolator = vtk.vtkPointInterpolator()
interpolator.SetSourceData(polyData)
interpolator.SetKernel(gaussian_kernel)
interpolator.Update()

# Schreibe das interpolierte VTK-Modell in eine Datei
output_filename = "test4_output.vtk"
writer = vtk.vtkPolyDataWriter()
writer.SetInputData(interpolator.GetOutput())
writer.SetFileName(output_filename)
writer.Write()