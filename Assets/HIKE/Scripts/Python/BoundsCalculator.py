from alive_progress import alive_bar

# Verzeichnis angeben
file = "D:/Windows/Desktop/data/reduced_harz_combined_out.txt"
out_file = "D:/Windows/Desktop/data/test_bounds.txt"

with open(out_file, "w") as out:
    num_lines = sum(1 for _ in open(file))

    minX = 20000000
    maxX = 0
    minY = 20000000
    maxY = 0
    minHeight = 50000
    maxHeight = 0

    # Datei öffnen und Zeilen einlesen
    with open(file, 'rb') as file:
            with alive_bar(num_lines) as inner_bar:
                for line in file:
                    coord = [float(num) for num in line.split()]

                    if coord[0] < minX:
                        minX = coord[0]

                    if coord[0] > maxX:
                        maxX = coord[0]

                    if coord[1] < minY:
                        minY = coord[1]

                    if coord[1] > maxY:
                        maxY = coord[1]

                    if coord[2] < minHeight:
                        minHeight = coord[2]

                    if coord[2] > maxHeight:
                        maxHeight = coord[2]

                    inner_bar()
    out.write("X: Min=%s Max=%s Diff=%s\n" % (minX, maxX, maxX-minX))
    out.write("Y: Min=%s Max=%s Diff=%s\n" % (minY, maxY, maxY-minY))
    out.write("Height: Min=%s Max=%s Diff=%s\n" % (minHeight, maxHeight, maxHeight-minHeight))
    file.close()
out.close()