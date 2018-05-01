using Prometeo.Planner.Console.Tools;
using Prometeo.Planner.Console.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for Subscribe.xaml
    /// </summary>
    public partial class Subscribe : Window
    {
        public ApplicationCommand CmdAccept { get; set; }
        public ApplicationCommand CmdCancel { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Subscribe()
        {
            InitializeComponent();

            DataContext = this;
            CmdAccept = new ApplicationCommand(CmdAccept_Execute);
            CmdCancel = new ApplicationCommand(CmdCancel_Execute);
        }

        private void CmdCancel_Execute(object obj)
        {
            this.Hide();
        }

        private void CmdAccept_Execute(object obj)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Phone))
                return;

            this.Hide();
        }

        public static bool AddNewUserToGroup(string group)
        {
            var wdw = new Subscribe();
            wdw.ShowDialog();

            if (string.IsNullOrEmpty(wdw.Name) || string.IsNullOrEmpty(wdw.Phone))
                return false;

            var result = RESTTools.SimpleURLRequest(ConfigurationManager.AppSettings["subscribeNumberToGroupAlert"].ToString(), wdw.Name, wdw.Phone, group);
            wdw.Close();

            return result;
        }
    }
}
