using Fluent;
using Prometeo.Planner.Console.Tools;
using Prometeo.Planner.Console.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Shapes;

namespace Prometeo.Planner.Console
{
    /// <summary>
    /// Interaction logic for FireViewer.xaml
    /// </summary>
    public partial class FireViewer : RibbonWindow, INotifyPropertyChanged
    {
        public ApplicationCommand CmdNotifyFire { get; set; }
        public ApplicationCommand CmdSafeFire { get; set; }
        public ApplicationCommand CmdNotAFire { get; set; }

        public string ImagePath { get; set; }
        public int ImageHeight { get; private set; }
        public int ImageWidth { get; private set; }
        public int ImageVPadding { get; private set; }
        public int ImageHPadding { get; private set; }
        public int TotalHeight { get => ImageHeight + ImageVPadding; }
        public int TotalWidth { get => ImageWidth + ImageHPadding; }
        public bool ShowTags { get => _showTags; set { _showTags = value; NotifyPropertyChanged("ShowTags"); renderBoxes(ImageResults, true); } }
        bool _showTags;

        public Thickness CanvasMargin => new Thickness(0, -ImageVPadding,0,0);
        public ImageDetectionResult ImageResults { get; private set; }
        public UserFeedbackAction ActionTaken { get; private set; }

        double _longitude, _latitude;
        static readonly int _imagepad = 850;
        public FireViewer(string imagePath, ImageDetectionResult results, double longitude, double latitude)
        {
            InitializeComponent();
            CmdNotifyFire = new ApplicationCommand(CmdNotifyFire_Execute);
            CmdSafeFire = new ApplicationCommand(CmdSafeFire_Execute);
            CmdNotAFire = new ApplicationCommand(CmdNotAFire_Execute);

            DataContext = this;
            ImagePath = imagePath;
            ImageResults = results;
            _longitude = longitude;
            _latitude = latitude;

            Bitmap img = new Bitmap(imagePath);

            ImageHeight = img.Height;
            ImageWidth = img.Width;

            double scale = 800.0 / Math.Max(ImageWidth, ImageHeight);
            ImageHeight = (int) Math.Ceiling(ImageHeight * scale);
            ImageWidth = (int)Math.Ceiling(ImageWidth * scale);
            if (ImageWidth > ImageHeight)
            {
                ImageHPadding = 0;
                ImageVPadding = (int)Math.Ceiling((double)(ImageWidth - ImageHeight) / 2);
            }
            else
            {
                ImageHPadding = (int)Math.Ceiling((double)(ImageHeight - ImageWidth) / 2);
                ImageVPadding = 0;
            }

            renderBoxes(results, false);

            NotifyPropertyChanged("CanvasMargin");
        }

        private void renderBoxes(ImageDetectionResult results, bool rerender)
        {
            if (rerender)
            {
                var image = MainCanvas.Children[0];
                MainCanvas.Children.Clear();
                MainCanvas.Children.Add(image);
            }

            var rect_scale = 800.0 / _imagepad;

            if (ShowTags)
                for (int i = 0; i < results.boxes.Count; i++)
                {
                    var box = results.boxes[i].Select((v) => Math.Max(0, Math.Min(_imagepad, v * rect_scale))).ToArray();
                    var rectangle = new System.Windows.Shapes.Rectangle();

                    rectangle.SetValue(Canvas.LeftProperty, box[0]);
                    rectangle.SetValue(Canvas.TopProperty, box[1]);
                    rectangle.Height = box[2] - box[0];
                    rectangle.Width = box[3] - box[1];
                    rectangle.Stroke = new SolidColorBrush() { Color = Colors.Red, Opacity = results.scores[i] };
                    MainCanvas.Children.Add(rectangle);

                    var score = new TextBlock()
                    {
                        Background = new SolidColorBrush() { Color = Colors.Red, Opacity = results.scores[i] },
                        FontWeight = FontWeights.Bold,
                        Height = 15,
                    };
                    score.SetValue(Canvas.LeftProperty, box[0]);
                    score.SetValue(Canvas.TopProperty, box[1] - score.Height);

                    score.Text = Math.Round(results.scores[i], 2).ToString();

                    MainCanvas.Children.Add(score);
                }
        }

        private void CmdNotAFire_Execute(object obj)
        {
            superviseImage("negative");
            MessageBox.Show("Thanks for your input about this image. Your feedback allows us to make our detection software better.", "Feedback", MessageBoxButton.OK, MessageBoxImage.Information);

            ActionTaken = UserFeedbackAction.NoFire;
            Hide();
        }

        private void CmdSafeFire_Execute(object obj)
        {
            superviseImage("positive");
            MessageBox.Show("Thanks for notifying a safe fire. Your feedback allows us to narrow our detection.", "Feedback", MessageBoxButton.OK, MessageBoxImage.Information);

            ActionTaken = UserFeedbackAction.SafeFire;
            Hide();
        }

        private void CmdNotifyFire_Execute(object obj)
        {
            if (MessageBox.Show("We are about to notify all members of the PrometheusAlert about this fire. We will send the location of the incident. Proceed?", "Send alerts", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                superviseImage("positive");
                RESTTools.SimpleGet(ConfigurationManager.AppSettings["sendAlertToGroupEndpoint"].ToString(), _latitude.ToString(), _longitude.ToString(), ConfigurationManager.AppSettings["subscribePartition"].ToString());

                ActionTaken = UserFeedbackAction.Alert;
                Hide();
            }
        }

        public bool superviseImage(string label)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["prometheusWebServiceUrl"].ToString());
                var response = client.PostAsync(string.Format("supervise/{1}/{0}", ImageResults.scoringId, label), null).Result;
                if (!response.IsSuccessStatusCode)
                    return false;

                return true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static UserFeedbackAction ViewFire(string imagePath, ImageDetectionResult results, double longitude, double latitude)
        {
            var wdw = new FireViewer(imagePath, results, longitude, latitude);
            wdw.ShowDialog();

            return wdw.ActionTaken;
        }
    }
}
