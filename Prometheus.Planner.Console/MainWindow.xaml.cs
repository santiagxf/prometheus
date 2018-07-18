using Fluent;
using Microsoft.Maps.MapControl.WPF;
using Prometeo.Planner.Console.Geo;
using Prometeo.Planner.Console.Map;
using Prometeo.Planner.Console.Tools;
using Prometeo.Planner.Console.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prometeo.Planner.Console
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow,  INotifyPropertyChanged
    {
        // OPTIONS
        public bool RenderRedFlagAlerts { get; set; }
        public bool RenderWeatherConditions { get; set; }

        public ApplicationCommand CmdEndPolygonDrawing { get; set; }
        public ApplicationCommand CmdStartPolygon { get; set; }
        public ApplicationCommand CmdCleanMap { get; set; }
        public ApplicationCommand CmdAnalizeNow { get; set; }
        public ApplicationCommand CmdMoveMap { get; set; }
        public ApplicationCommand CmdNotificationOpened { get; set; }
        public ApplicationCommand CmdAlertGroups { get; set; }
        public ApplicationCommand CmdConfig { get; set; }
        public ApplicationCommand CmdSave { get; set; }
        public ApplicationCommand OpenSavedAOI { get; set; }
        public ApplicationCommand CmdDismissRedFlagNotifications { get; set; }


        public AreasOfInterestCollection SavedAreasOfInterest { get; set; }
        public FlightCollection Flights { get; set; }
        public AreasOfInterestCollection AreasOfInterest { get; set; }
        public AreasOfInterestCollection RedFlagsNotifications { get; set; }
        public bool DrawingRequested { get; set; }
        public Visibility AnyRedFlagsNotification { get; set; }
        public Visibility FireAlert { get; set; }
        public Visibility AreaIsGood { get; set; }
        public Visibility DrawingHelpVisible => DrawingRequested ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AddToMyAreasVisibility { get; set; }

        MapModel _draftMapModel;
        
        static readonly int MAP_ZOOM_LEVEL_AREA = 16;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            RenderRedFlagAlerts = true;
            RenderWeatherConditions = false;

            CmdEndPolygonDrawing = new ApplicationCommand(CmdEndPolygonDrawing_Execute);
            CmdStartPolygon = new ApplicationCommand(CmdStartPolygon_Execute);
            CmdCleanMap = new ApplicationCommand(CmdCleanMap_Execute);
            CmdAnalizeNow = new ApplicationCommand(CmdAnalizeNow_Execute);
            CmdMoveMap = new ApplicationCommand(CmdMoveMap_Execute);
            CmdNotificationOpened = new ApplicationCommand(CmdNotificationOpened_Execute);
            CmdAlertGroups = new ApplicationCommand(CmdAlertGroups_Execute);
            CmdConfig = new ApplicationCommand(CmdConfig_Execute);
            CmdSave = new ApplicationCommand(CmdSave_Execute);
            OpenSavedAOI = new ApplicationCommand(OpenSavedAOI_Execute);
            CmdDismissRedFlagNotifications = new ApplicationCommand(CmdDismissRedFlagNotifications_Execute);


            Flights = new FlightCollection();
            RedFlagsNotifications = new AreasOfInterestCollection();
            AreasOfInterest = new AreasOfInterestCollection();
            SavedAreasOfInterest = new AreasOfInterestCollection();

            UpdateRedFlagNotifications(Settings.RED_FLAGS_COUNTRY_CODE);
            CmdCleanMap_Execute(null);
        }

        private void OpenSavedAOI_Execute(object aoi)
        {
            if (aoi is AreaOfInterest aoiObj)
            {
                if (!AreasOfInterest.Any((a) => a.Name == aoiObj.Name))
                {
                    CmdCleanMap_Execute(null);
                    AreasOfInterest.Add(aoiObj);
                    Flights.AddRange(aoiObj.Flights);
                }

                NotifyPropertyChanged("AreasOfInterest");
                NotifyPropertyChanged("Flights");
                RenderMap();
            }
        }

        private void CmdSave_Execute(object obj)
        {
            string name = AddFlightToHistory.PickUpName();
            if (!string.IsNullOrEmpty(name))
            {
                var area = AreasOfInterest.First();
                area.Name = name.ToUpper();

                SavedAreasOfInterest.Add(area);
                NotifyPropertyChanged("SavedAreasOfInterest");
            }
        }

        private void CmdDismissRedFlagNotifications_Execute(object obj)
        {
            AnyRedFlagsNotification = Visibility.Collapsed;
            NotifyPropertyChanged("AnyRedFlagsNotification");
        }

        private void UpdateRedFlagNotifications(string countryCode)
        {
            RedFlagsNotifications.Clear();

            var redFlags = new AreaOfInterest()
            {
                MapModel = new MapModel(BmpPlanner)
                {
                    Shading = MapPolygonExtensions.REDFLAG_AREA_SHADING,
                    Stroke = MapPolygonExtensions.REDFLAG_AREA_STROKE
                }
            };

            switch (countryCode)
            {
                case "UNITED STATES":
                    var fireAlerts = NationalWeatherService.QueryFireAlerts(NationalWeatherService.RED_FLAG_ALERT, countryCode);
                    if (fireAlerts.Any())
                        NationalWeatherService.ConvertUGCtoPolygon(fireAlerts, redFlags.MapModel);
                    break;
                case "ARGENTINA":
                    NationalWeatherService.GetAllPolygonsFromShape(redFlags.MapModel, "ar");
                    break;
            }

            AnyRedFlagsNotification = redFlags.MapModel.Polygons.Any() ? Visibility.Visible : Visibility.Collapsed;
            if (AnyRedFlagsNotification == Visibility.Visible)
                RedFlagsNotifications.Add(redFlags);

            NotifyPropertyChanged("AnyRedFlagsNotification");
            NotifyPropertyChanged("RedFlagsNotifications");
        }

        private void CmdConfig_Execute(object obj)
        {
            (new Settings()).ShowDialog();
        }

        private void CmdAlertGroups_Execute(object obj)
        {
            (new AlertGroup()).ShowDialog();
        }

        private void CmdNotificationOpened_Execute(object obj)
        {
            FireAlert = Visibility.Collapsed;
            NotifyPropertyChanged("FireAlert");
        }

        private void CmdMoveMap_Execute(object obj)
        {
            var pos = ResolveLocation.Resolve(false);
            if (pos != null)
                BmpPlanner.SetView(new Location(pos[0], pos[1]), 10);
        }

        private void CmdAnalizeNow_Execute(object obj)
        {
            var filePath = DialogTools.ShowFolderPickerDialog("Select the images folder");

            if (!string.IsNullOrEmpty(filePath))
            {
                var wdw = new AnalyzeImages()
                {
                    FilePath = filePath
                };
                wdw.ShowDialog();

                var missingLocations = wdw.DetectedFires.Where((f) => f.Longitude == 0 && f.Latitude == 0);
                if (missingLocations.Any())
                {
                    var imageLocation = ResolveLocation.Resolve(true);
                    if (imageLocation != null)
                    {
                        foreach (var l in missingLocations)
                        { 
                            l.Latitude = imageLocation[0];
                            l.Longitude = imageLocation[1];
                        }
                    }
                }

                var flight = new Flight(wdw.DetectedFires, wdw.CoveredArea, filePath, new MapModel(BmpPlanner));
                Flights.Add(flight);
                NotifyPropertyChanged("Flights");

                AreasOfInterest.AddToAnalizedArea(flight);
                AreasOfInterest.NotifyAllPropertyChanged("AnalizedAreaPercentage");
                NotifyPropertyChanged("AreasOfInterest");

                wdw.Close();
                RenderMap();
            }
        }

        private void CmdCleanMap_Execute(object obj)
        {
            FireAlert = Visibility.Collapsed;
            AreaIsGood = Visibility.Collapsed;

            NotifyPropertyChanged("FireAlert");
            NotifyPropertyChanged("AreaIsGood");

            Flights.Clear();
            AreasOfInterest.Clear();

            NotifyPropertyChanged("Flights");
            NotifyPropertyChanged("AreasOfInterest");

            AddToMyAreasVisibility = Visibility.Hidden;
            NotifyPropertyChanged("AddToMyAreasVisibility");

            RenderMap();
        }

        private void CmdStartPolygon_Execute(object obj)
        {
            if (BmpPlanner.ZoomLevel > 10 || MessageBox.Show("The current zoom level in the map looks a big high. We suggest you use the Zoom and Pan to look for the area of interest by name before marking it. Do you want to continue anyway?", "Zoom level too high", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _draftMapModel = new MapModel(BmpPlanner);

                _draftMapModel.Shading = MapPolygonExtensions.INTEREST_AREA_SHADING;
                _draftMapModel.Stroke = MapPolygonExtensions.INTEREST_AREA_STROKE;

                DrawingRequested = true;
                NotifyPropertyChanged("DrawingRequested");
                NotifyPropertyChanged("DrawingHelpVisible");

                BmpPlanner.MouseDown += BmpPlanner_MouseDown;
            }
        }

        private void RenderMap()
        {
            BmpPlanner.Children.Clear();

            if (RenderWeatherConditions)
                BmpPlanner.Children.Add(new MapTileLayer()
                {
                    TileSource = new WMSTileSource(),
                    Opacity = 0.8
                });

            MapLayer labelLayer = new MapLayer();

            int totalFireCount = 0;

            if (_draftMapModel != null)
                RenderMap(_draftMapModel, labelLayer, null, null);

            foreach (var aoi in AreasOfInterest)
                RenderMap(aoi.MapModel, labelLayer, null, null);

            foreach (var flight in Flights)
                totalFireCount += RenderMap(flight.MapModel, labelLayer, "Potential fire", (ControlTemplate)Application.Current.Resources["FirePushPinTemplate"]);

            if (RenderRedFlagAlerts)
                foreach (var redflag in RedFlagsNotifications)
                    RenderMap(redflag.MapModel, labelLayer, "Red flag warning", null);

            if (totalFireCount > 0)
            {
                var centerPoint = Flights.Where((f) => f.FireDetected).First().MapModel.Marks.First();
                BmpPlanner.SetView(new Location(centerPoint.Latitude, centerPoint.Longitude), Math.Max(MAP_ZOOM_LEVEL_AREA, BmpPlanner.ZoomLevel));

                FireAlert = Visibility.Visible;
                AreaIsGood = Visibility.Collapsed;
                NotifyPropertyChanged("AreaIsGood");
                NotifyPropertyChanged("FireAlert");
            }
            else if (Flights.Any())
            {
                var centerPoint = Flights.First().MapModel.Polygons.First().CenterPosition();
                BmpPlanner.SetView(new Location(centerPoint.Latitude, centerPoint.Longitude), Math.Max(MAP_ZOOM_LEVEL_AREA, BmpPlanner.ZoomLevel));

                AreaIsGood = Visibility.Visible;
                FireAlert = Visibility.Collapsed;
                NotifyPropertyChanged("AreaIsGood");
                NotifyPropertyChanged("FireAlert");
            }
            else if (AreasOfInterest.Any())
            {
                var centerPoint = AreasOfInterest.First().MapModel.Polygons.First().CenterPosition();
                BmpPlanner.SetView(new Location(centerPoint.Latitude, centerPoint.Longitude), Math.Max(MAP_ZOOM_LEVEL_AREA, BmpPlanner.ZoomLevel));
            }

            BmpPlanner.Children.Add(labelLayer);
        }

        private int RenderMap(MapModel mapModel, MapLayer labelLayer, string pinToolTipText, ControlTemplate pushpinTemplate)
        {
            foreach (var polygon in mapModel.Polygons)
            {
                BmpPlanner.Children.Add(polygon);
                //labelLayer.AddChild(AddLabelInMap(polygon.CoveredArea().ToString("n2") + " m2"), polygon.CenterPosition());
            }

            int index = 0;
            foreach (var loc in mapModel.Marks)
            {
                var pin = new Pushpin();
                if (pushpinTemplate != null)
                    pin.Template = pushpinTemplate;
                if (!string.IsNullOrEmpty(loc.ImagePath))
                    pin.MouseDown += (sender, e) => Pin_MouseDown(loc, e);
                pin.Location = loc;
                pin.ToolTip = pinToolTipText;
                pin.Content = ++index;
                BmpPlanner.Children.Add(pin);
            }

            return index;
        }

        private void Pin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var location = (LocationMark)sender;

            switch (FireViewer.ViewFire(location.ImagePath, location.DetectionResults, location.Longitude, location.Latitude))
            {
                case UserFeedbackAction.Alert:
                    MessageBox.Show("Your team has been notified about it. Coordinates of the fire were included", "Notified", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case UserFeedbackAction.NoFire:
                    Flights.RemoveMark(location);
                    RenderMap();
                    break;
            }
        }

        private Label AddLabelInMap(string text)
        {
            return new Label()
            {
                Content = text,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };
        }

        private void CmdEndPolygonDrawing_Execute(object obj)
        {
            if (_draftMapModel != null && _draftMapModel.IsCurrentlyDrawing)
            {
                var pol = _draftMapModel.FinishCurrentPolygon();

                BmpPlanner.MouseDown -= BmpPlanner_MouseDown;
                BmpPlanner.MouseMove -= BmpPlanner_MouseMove;

                DrawingRequested = false;
                NotifyPropertyChanged("DrawingRequested");
                NotifyPropertyChanged("DrawingHelpVisible");

                var areaInAcres = pol.CoveredArea();

                AreasOfInterest.Add(new AreaOfInterest()
                {
                    AnalizedArea = 0,
                    Area = areaInAcres,
                    RequiredFlightTime = Math.Round(areaInAcres * 1.0425 / 6, 2),
                    RequiredImages = (int) Math.Ceiling(areaInAcres * 37 / 6),
                    MapModel = _draftMapModel
                });
                NotifyPropertyChanged("AreasOfInterest");

                AddToMyAreasVisibility = Visibility.Visible;
                NotifyPropertyChanged("AddToMyAreasVisibility");

                _draftMapModel = null;

                RenderMap();
            }
        }

        private void BmpPlanner_MouseDown(object sender, MouseEventArgs e)
        {
            if (_draftMapModel == null)
            {
                // If null, something is wrong
                BmpPlanner.MouseDown -= BmpPlanner_MouseDown;
                BmpPlanner.MouseMove -= BmpPlanner_MouseMove;
            }
            else
            {
                if (!_draftMapModel.IsCurrentlyDrawing)
                {
                    _draftMapModel.StartNewPolygon();
                    BmpPlanner.MouseMove += BmpPlanner_MouseMove;
                }

                _draftMapModel.AddPointToPolygon(e.GetPosition(BmpPlanner));
                RenderMap();
            }
        }

        private void BmpPlanner_MouseMove(object sender, MouseEventArgs e)
        {
            _draftMapModel.AddDraftPointToPolygon(e.GetPosition(BmpPlanner));
            RenderMap();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
