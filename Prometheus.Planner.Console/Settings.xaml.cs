using Prometeo.Planner.Console.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public static int IMAGE_BATCH_SIZE = 10;
        public static double SCORE_SENSITIVITY = 0.5;
        public static double RESIZE_IMAGES_FACTOR = 0.5;
        public static string RED_FLAGS_COUNTRY_CODE = "UNITED STATES";

        public ApplicationCommand CmdOk { get; set; }
        public ApplicationCommand CmdCancel { get; set; }
        public ApplicationCommand CmdSubscription { get; set; }
        public int BatchSize { get; set; }
        public double Sensitivity { get; set; }
        public double FlyingAltitude { get; set; }
        public double AverageFlyingSpeed { get; set; }
        public double CameraAngle { get; set; }
        public double ResizeFactor { get; set; }
        public string RedFlagAlertsCountry { get; set; }
        bool _windowCanceled { get; set; }
        public Settings()
        {
            InitializeComponent();
            DataContext = this;

            Sensitivity = SCORE_SENSITIVITY;
            BatchSize = IMAGE_BATCH_SIZE;
            ResizeFactor = RESIZE_IMAGES_FACTOR;
            RedFlagAlertsCountry = RED_FLAGS_COUNTRY_CODE;

            FlyingAltitude = 400;
            AverageFlyingSpeed = 27;
            CameraAngle = 90;

            CmdOk = new ApplicationCommand(CmdOk_Execute);
            CmdCancel = new ApplicationCommand(CmdCancel_Execute);
            CmdSubscription = new ApplicationCommand(CmdSubscription_Execute);
        }

        private void CmdSubscription_Execute(object obj)
        {
            (new AlertGroup()).ShowDialog();
        }

        private void CmdCancel_Execute(object obj)
        {
            _windowCanceled = true;
            Close();
        }

        private void CmdOk_Execute(object obj)
        {
            SCORE_SENSITIVITY = Sensitivity;
            IMAGE_BATCH_SIZE = BatchSize;
            RESIZE_IMAGES_FACTOR = ResizeFactor;
            RED_FLAGS_COUNTRY_CODE = RedFlagAlertsCountry;

            Close();
        }
    }
}
