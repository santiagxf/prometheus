using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Prometeo.Planner.Console.Tools
{
    class DialogTools
    {
        public static string ShowFilePickerDialog(string title)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image (*.jpg)|*.jpg";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            return result == true ? dlg.FileName : string.Empty;
        }

        public static string ShowFolderPickerDialog(string title)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return fbd.SelectedPath;
                else
                    return string.Empty;
            }
        }
    }
}
