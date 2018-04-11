using Fluent;
using Microsoft.Maps.MapControl.WPF;
using Prometeo.Planner.Console.Geo;
using Prometeo.Planner.Console.Map;
using Prometeo.Planner.Console.Tools;
using Prometeo.Planner.Console.ViewModel;
using System;
using System.Collections.Generic;
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
        public ApplicationCommand CmdEndPolygonDrawing { get; set; }
        public ApplicationCommand CmdStartPolygon { get; set; }
        public ApplicationCommand CmdCleanMap { get; set; }
        public ApplicationCommand CmdAnalizeNow { get; set; }
        public ApplicationCommand CmdMoveMap { get; set; }
        public ApplicationCommand CmdNotificationOpened { get; set; }

        public MapModel MapModel { get; set; }
        public bool DrawingRequested { get; set; }
        public Visibility FireAlert { get; set; }
        public Visibility AreaIsGood { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            CmdEndPolygonDrawing = new ApplicationCommand(CmdEndPolygonDrawing_Execute);
            CmdStartPolygon = new ApplicationCommand(CmdStartPolygon_Execute);
            CmdCleanMap = new ApplicationCommand(CmdCleanMap_Execute);
            CmdAnalizeNow = new ApplicationCommand(CmdAnalizeNow_Execute);
            CmdMoveMap = new ApplicationCommand(CmdMoveMap_Execute);
            CmdNotificationOpened = new ApplicationCommand(CmdNotificationOpened_Execute);

            MapModel = new MapModel(BmpPlanner);
            MapModel.Shading = new SolidColorBrush(Color.FromArgb(0x50, 0x2B, 0x57, 0x9A));
            MapModel.Stroke = new SolidColorBrush(Color.FromRgb(0x2B, 0x57, 0x9A));
            MapModel.StrokeThickness = 2;

            FireAlert = Visibility.Collapsed;
            AreaIsGood = Visibility.Collapsed;
        }

        private void CmdNotificationOpened_Execute(object obj)
        {
            throw new NotImplementedException();
        }

        private void CmdMoveMap_Execute(object obj)
        {
            var pos = ResolveLocation.Resolve();
            if (pos != null)
            {
                BmpPlanner.SetView(new Location(pos[0], pos[1]), 8);
            }
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
                    var imageLocation = ResolveLocation.Resolve();
                    if (imageLocation != null)
                    {
                        foreach (var l in missingLocations)
                        { 
                            l.Latitude = imageLocation[0];
                            l.Longitude = imageLocation[1];
                        }
                    }
                }

                var allCoveredArea = wdw.CoveredArea.ConvexHull();
                if (allCoveredArea != null && allCoveredArea.Count > 2)
                {
                    MapModel.StartNewPolygon();

                    foreach (var point in allCoveredArea)
                        MapModel.AddPointToPolygon(point);

                    MapModel.FinishCurrentPolygon();
                }


                foreach (var loc in wdw.DetectedFires)
                    MapModel.Marks.Add(loc);

                wdw.Close();
                RenderMap();
            }
        }

        private void CmdCleanMap_Execute(object obj)
        {
            MapModel.Clean();
            RenderMap();
        }

        private void CmdStartPolygon_Execute(object obj)
        {
            DrawingRequested = true;
            NotifyPropertyChanged("DrawingRequested");

            BmpPlanner.MouseDown += BmpPlanner_MouseDown;
        }

        private void RenderMap()
        {
            BmpPlanner.Children.Clear();
            MapLayer labelLayer = new MapLayer();

            foreach (var polygon in MapModel.Polygons)
            {
                BmpPlanner.Children.Add(polygon);
                labelLayer.AddChild(AddLabelInMap(polygon.CoveredArea().ToString("n2") + " m2"), polygon.CenterPosition());
            }

            int index = 0;
            foreach (var loc in MapModel.Marks)
            {
                var pin = new Pushpin();
                pin.MouseDown += (sender, e) => Pin_MouseDown(loc, null);
                pin.Location = loc;
                pin.ToolTip = "Potential fire";
                pin.Content = index++;
                BmpPlanner.Children.Add(pin);
            }

            BmpPlanner.Children.Add(labelLayer);

            if (MapModel.Marks.Any())
            {
                var centerPoint = MapModel.Marks.First();
                BmpPlanner.SetView(new Location(centerPoint.Latitude, centerPoint.Longitude), 8);

                FireAlert = Visibility.Visible;
                AreaIsGood = Visibility.Collapsed;
                NotifyPropertyChanged("AreaIsGood");
                NotifyPropertyChanged("FireAlert");
            }
            else
            {
                AreaIsGood = Visibility.Collapsed;
                FireAlert = Visibility.Visible;
                NotifyPropertyChanged("AreaIsGood");
                NotifyPropertyChanged("FireAlert");
            }
        }

        private void Pin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var location = (LocationMark)sender;

            var wdw = new FireViewer(location.ImagePath, location.DetectionResults);
            wdw.ShowDialog();
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
            if (MapModel.IsCurrentlyDrawing)
            {
                MapModel.FinishCurrentPolygon();

                BmpPlanner.MouseDown -= BmpPlanner_MouseDown;
                BmpPlanner.MouseMove -= BmpPlanner_MouseMove;

                DrawingRequested = false;
                NotifyPropertyChanged("DrawingRequested");

                RenderMap();
            }
        }

        private void BmpPlanner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!MapModel.IsCurrentlyDrawing)
            {
                MapModel.StartNewPolygon();
                BmpPlanner.MouseMove += BmpPlanner_MouseMove;
            }

            MapModel.AddPointToPolygon(e.GetPosition(BmpPlanner));
            RenderMap();
        }

        private void BmpPlanner_MouseMove(object sender, MouseEventArgs e)
        {
            MapModel.AddDraftPointToPolygon(e.GetPosition(BmpPlanner));
            RenderMap();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
