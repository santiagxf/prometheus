using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.ViewModel.Alerts
{
    public class AlertGroups : TableEntity
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
