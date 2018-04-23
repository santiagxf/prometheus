using Microsoft.Maps.MapControl.WPF;
using Prometeo.Planner.Console.Geo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Prometeo.Planner.Console.Map
{
    public class MapModel
    {
        MapPolygon _currentDrawingActivity;
        Location _currentDraftLoc;

        Microsoft.Maps.MapControl.WPF.Map _map;
        public Collection<MapPolygon> Polygons { get; set; }
        public GeoCoordinateCollection Marks { get; set; }
        public SolidColorBrush Shading { get; set; }
        public SolidColorBrush Stroke { get; set; }
        public int StrokeThickness { get; set; }

        public MapModel(Microsoft.Maps.MapControl.WPF.Map map)
        {
            Polygons = new Collection<MapPolygon>();
            Marks = new GeoCoordinateCollection();
            _map = map;
        }

        public bool IsCurrentlyDrawing
        {
            get => _currentDrawingActivity != null;
        }

        public bool IsCurrentlyDraftLocation
        {
            get => _currentDraftLoc != null;
        }

        public void Clean()
        {
            Polygons.Clear();
            Marks.Clear();
        }

        public void StartNewPolygon()
        {
            _currentDrawingActivity = new MapPolygon()
            {
                Locations = new LocationCollection(),
                StrokeThickness = StrokeThickness,
                Stroke = Stroke,
                Fill = Shading
            };
            Polygons.Add(_currentDrawingActivity);
        }

        public void AddPointToPolygon(Point point)
        {
            if (!IsCurrentlyDraftLocation)
                _currentDrawingActivity.Locations.Add(_map.ViewportPointToLocation(point));

            _currentDraftLoc = null;
        }

        public void AddPointToPolygon(Location location)
        {
            if (!IsCurrentlyDraftLocation)
                _currentDrawingActivity.Locations.Add(location);

            _currentDraftLoc = null;
        }

        public void FinishCurrentPolygon()
        {
            if (IsCurrentlyDraftLocation)
            {
                if (_currentDrawingActivity.Locations.Count == 2)
                    Polygons.RemoveAt(Polygons.Count - 1);
                else
                    _currentDrawingActivity.Locations.RemoveAt(_currentDrawingActivity.Locations.Count - 1);
            }

            _currentDrawingActivity = null;
            _currentDraftLoc = null;
        }

        internal void AddDraftPointToPolygon(Point point)
        {
            if (!IsCurrentlyDraftLocation)
            {
                _currentDraftLoc = _map.ViewportPointToLocation(point);
                _currentDrawingActivity.Locations.Add(_currentDraftLoc);
            }
            else
                _currentDrawingActivity.Locations[_currentDrawingActivity.Locations.Count - 1] = _map.ViewportPointToLocation(point);
        }
    }
}
