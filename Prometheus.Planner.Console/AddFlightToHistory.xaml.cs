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
    /// Interaction logic for AddFlightToHistory.xaml
    /// </summary>
    public partial class AddFlightToHistory : Window
    {
        public ApplicationCommand CmdAccept { get; set; }
        public ApplicationCommand CmdCancel { get; set; }
        public string AreaName { get; set; }

        public AddFlightToHistory()
        {
            DataContext = this;
            CmdAccept = new ApplicationCommand(CmdAccept_Execute);
            CmdCancel = new ApplicationCommand(CmdCancel_Execute);

            InitializeComponent();
        }

        private void CmdCancel_Execute(object obj)
        {
            AreaName = null;
            this.Hide();
        }

        private void CmdAccept_Execute(object obj)
        {
            this.Hide();
        }

        internal static string PickUpName()
        {
            var wdw = new AddFlightToHistory();
            wdw.ShowDialog();

            var result = wdw.AreaName;
            wdw.Close();

            return result;
        }
    }
}
