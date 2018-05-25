using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;
using Prometeo.Planner.Console.Geo;
using Prometeo.Planner.Console.Map;

namespace Prometeo.Planner.Console.ViewModel
{
    public class Flight
    {
        public Flight(Collection<LocationMark> detectedFires, GeoCoordinateCollection coveredArea, string filePath, MapModel mapModel)
        {
            Folder = filePath;
            MapModel = mapModel;
            Images = coveredArea.Count;
            Date = DateTime.Now;

            if (MapModel != null)
            {
                if (detectedFires.Any())
                {
                    MapModel.Shading = MapPolygonExtensions.RED_AREA_SHADING;
                    MapModel.Stroke = MapPolygonExtensions.RED_AREA_STROKE;
                }
                else
                {
                    MapModel.Shading = MapPolygonExtensions.GREEN_AREA_SHADING;
                    MapModel.Stroke = MapPolygonExtensions.GREEN_AREA_STROKE;
                }

                var allCoveredArea = coveredArea.ConvexHull();

                MapPolygon analizedArea = null;
                if (allCoveredArea != null && allCoveredArea.Count > 2)
                {
                    MapModel.StartNewPolygon();

                    foreach (var point in allCoveredArea)
                        MapModel.AddPointToPolygon(point);

                    analizedArea = MapModel.FinishCurrentPolygon();
                    CoveredArea = analizedArea.CoveredArea();
                }

                foreach (var loc in detectedFires)
                    MapModel.Marks.Add(loc);
            }
        }

        public void RemoveMark(LocationMark location)
        {
            MapModel.Marks.Remove(MapModel.Marks.Where((m) => m.DetectionResults.scoringId == location.DetectionResults.scoringId).First());

            if (MapModel.Marks.Any())
            {
                MapModel.Shading = MapPolygonExtensions.RED_AREA_SHADING;
                MapModel.Stroke = MapPolygonExtensions.RED_AREA_STROKE;
            }
            else
            {
                MapModel.Shading = MapPolygonExtensions.GREEN_AREA_SHADING;
                MapModel.Stroke = MapPolygonExtensions.GREEN_AREA_STROKE;
            }
        }

        public string Folder { get; set; }
        public DateTime Date { get; set; }
        public int Images { get; set; }
        public double CoveredArea { get; set; }
        public bool FireDetected => MapModel.Marks.Count > 0;
        public string ResultAsString => FireDetected ? "At risk" : "Clean";
        public SolidColorBrush AreaColor => FireDetected ? MapPolygonExtensions.RED_AREA_SHADING : MapPolygonExtensions.GREEN_AREA_SHADING;
        public MapModel MapModel { get; set; }
    }
}
