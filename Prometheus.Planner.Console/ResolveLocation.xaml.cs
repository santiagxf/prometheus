using Microsoft.Maps.MapControl.WPF;
using Prometeo.Planner.Console.Geo;
using Prometeo.Planner.Console.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
    /// Interaction logic for ResolveLocation.xaml
    /// </summary>
    public partial class ResolveLocation : Window
    {
        public ApplicationCommand CmdAccept { get; set; }
        public ApplicationCommand CmdCancel { get; set; }
        public string Location { get; set; }
        public double[] ResolvedLocation { get; set; }

        public ResolveLocation()
        {
            DataContext = this;
            CmdAccept = new ApplicationCommand(CmdAccept_Execute);
            CmdCancel = new ApplicationCommand(CmdCancel_Execute);

            InitializeComponent();
        }

        private void CmdCancel_Execute(object obj)
        {
            this.Hide();
        }

        private void CmdAccept_Execute(object obj)
        {
            double lon, lat;
            BingMapsRestServices.FindLocationByName(Location, out lon, out lat);

            if (lon == 0 && lat == 0)
            {
                MessageBox.Show("We could find the location you mentioned");
                return;
            }

            ResolvedLocation = new double[] { lat, lon };
            this.Hide();
        }

        internal static double[] Resolve(bool gpsDataNotFound)
        {
            var wdw = new ResolveLocation();

            if (!gpsDataNotFound)
            {
                wdw.LblTitle.Content = "Look for a location to zoom in".ToUpper();
                wdw.LblText.Content = "Type the name of a city or state";
            }

            wdw.ShowDialog();

            var result = wdw.ResolvedLocation;
            wdw.Close();

            return result;
        }
    }
}
