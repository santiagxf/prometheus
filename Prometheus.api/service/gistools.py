import shapefile
import os

__workingDir = os.path.dirname(os.path.abspath(__file__))
__r = shapefile.Reader(os.path.join(__workingDir, r'./../gis/us/fz25jn18.shp'))

def getCenterFromUGC(ugc):
    state, zone = ugc[:2], ugc[3:]
    print('\tLooking for coordinates for', state, zone)

    for row in __r.shapeRecords():
        if (row.record[0] == state and row.record[1] == zone):
            return row.record[7], row.record[8]
    else:
        return 0,0

def getShapeFromUGC(ugc):
    state, zone = ugc[:2], ugc[3:]
    print('\tLooking for shape coordinates for', state, zone)

    for row in __r.shapeRecords():
        if (row.record[0] == state and row.record[1] == zone):
            return row.shape.__geo_interface__['coordinates'][0]
    else:
        return 0,0

