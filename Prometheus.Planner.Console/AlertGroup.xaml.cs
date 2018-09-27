using Newtonsoft.Json;
using Prometeo.Planner.Console.Tools;
using Prometeo.Planner.Console.ViewModel;
using Prometeo.Planner.Console.ViewModel.Alerts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Prometeo.Planner.Console
{
    /// <summary>
    /// Interaction logic for AlertGroup.xaml
    /// </summary>
    public partial class AlertGroup : Fluent.RibbonWindow, INotifyPropertyChanged
    {
        public ApplicationCommand CmdActivated { get; set; }
        public ApplicationCommand CmdAdd { get; set; }
        public ApplicationCommand CmdRemove { get; set; }
        public ApplicationCommand CmdCancel { get; set; }
        public List<AlertGroups> AllMembers { get; set; }
        public AlertGroups SelectedMember { get; set; }
        public Visibility AllMembersVisibility { get; set; }

        BackgroundWorker _worker;
        public AlertGroup()
        {
            InitializeComponent();
            DataContext = this;

            CmdAdd = new ApplicationCommand(CmdAdd_Execute);
            CmdRemove = new ApplicationCommand(CmdRemove_Execute);
            CmdCancel = new ApplicationCommand(CmdCancel_Execute);

            _worker = new BackgroundWorker();
            _worker.DoWork += worker_DoWork;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AllMembersVisibility = Visibility.Visible;
            NotifyPropertyChanged("AllMembersVisibility");
            NotifyPropertyChanged("AllMembers");
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<AlertGroups> members = new List<AlertGroups>();
            try
            {
                var partition = ConfigurationManager.AppSettings["subscribePartition"].ToString();
                var currentMembers = RESTTools.Get(ConfigurationManager.AppSettings["subscribtionMembers"].ToString(), partition);

                if (string.IsNullOrEmpty(currentMembers))
                    members = JsonConvert.DeserializeObject<IEnumerable<AlertGroups>>(currentMembers)?.ToList();

                AllMembers = members;
            }
            catch { }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            AllMembersVisibility = Visibility.Hidden;
            NotifyPropertyChanged("AllMembersVisibility");

            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private void CmdCancel_Execute(object obj)
        {
            Close();
        }

        private void CmdRemove_Execute(object obj)
        {
            if (SelectedMember != null)
                if (RESTTools.SimpleGet(ConfigurationManager.AppSettings["unsubscribeNumberToGroupAlert"].ToString(), SelectedMember.RowKey))
                    Window_Activated(null, null);
        }

        private void CmdAdd_Execute(object obj)
        {
            var partition = ConfigurationManager.AppSettings["subscribePartition"].ToString();

            if (Subscribe.AddNewUserToGroup(partition))
                Window_Activated(null, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
