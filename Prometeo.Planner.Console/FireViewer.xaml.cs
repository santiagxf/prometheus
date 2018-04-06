using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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
using System.Windows.Shapes;

namespace Prometeo.Planner.Console
{
    /// <summary>
    /// Interaction logic for FireViewer.xaml
    /// </summary>
    public partial class FireViewer : Window, INotifyPropertyChanged
    {
        public string ImagePath { get; set; }
        public int ImageHeight { get; private set; }
        public int ImageWidth { get; private set; }
        public int ImageVPadding { get; private set; }
        public int ImageHPadding { get; private set; }
        public int TotalHeight { get => ImageHeight + ImageVPadding; }
        public int TotalWidth { get => ImageWidth + ImageHPadding; }


        static readonly int _imagepad = 850;
        public FireViewer(string imagePath, ImageDetectionResult results)
        {
            InitializeComponent();

            DataContext = this;
            ImagePath = imagePath;

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
            var rect_scale = 800.0 / _imagepad;


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
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
