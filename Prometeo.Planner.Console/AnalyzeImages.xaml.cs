﻿using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Prometeo.Planner.Console.Geo;
using Prometeo.Planner.Console.Map;
using Prometeo.Planner.Console.Tools;
using Prometeo.Planner.Console.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
    /// Interaction logic for AnalyzeImages.xaml
    /// </summary>
    public partial class AnalyzeImages : Window, INotifyPropertyChanged
    {
        public ApplicationCommand CmdAccept { get; set; }
        public ApplicationCommand CmdCancel { get; set; }
        public string CurrentStatus { get; set; }
        public bool ServiceRunning { get; set; }
        public string FilePath { get; set; }
        BackgroundWorker _worker;
        string _serviceUrl;


        public Collection<LocationMark> DetectedFires { get; set; }

        public AnalyzeImages()
        {
            InitializeComponent();

            ServiceRunning = true;
            DetectedFires = new Collection<LocationMark>();

            _serviceUrl = ConfigurationManager.AppSettings["prometheusWebServiceUrl"].ToString();
            _worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.ProgressChanged += _worker_ProgressChanged;

            DataContext = this;
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentStatus = (string) e.UserState;
            NotifyPropertyChanged("CurrentStatus");
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!ServiceRunning)
                MessageBox.Show("The Prometheus web service seems not to be running. Has the address of the service changed?", "Service", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            Hide();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int fileIndex = 1;
            foreach (var file in Directory.GetFiles(FilePath))
                if (ServiceRunning && System.IO.Path.GetExtension(file).ToLower() == ".jpg")
                {
                    _worker.ReportProgress(fileIndex++, file);

                    var result = scoreSingleImage(file);
                    var fires = result.labels.Where((l) => l == 1);
                    if (fires.Count() > 0)
                    {
                        var imageLocation = BingMapsRestServices.GetLatLongFromImage(file);
                        if (imageLocation == null)
                            imageLocation = new double[] { 0, 0 };
                        DetectedFires.Add(new LocationMark(imageLocation[0], imageLocation[1], file, filterResults(result)));
                    }
                }
        }

        private ImageDetectionResult filterResults(ImageDetectionResult result)
        {
            var filtered = new ImageDetectionResult()
            {
                executionTimeMs = result.executionTimeMs
            };

            var minScore = result.scores.Min();
            if (minScore > 0)
                minScore = minScore * 0.5;

            for (var i = 0; i < result.labels.Count; i++)
            {
                if (result.labels[i] == 1 && result.scores[i] > minScore)
                {
                    filtered.boxes.Add(result.boxes[i]);
                    filtered.scores.Add(result.scores[i]);
                }
            }

            return filtered;
        }

        public ImageDetectionResult scoreSingleImage(string imagePath)
        {
            byte[] data = File.ReadAllBytes(imagePath);

            // Generate post objects
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("filename", "Image.jpg");
            postParameters.Add("fileformat", "jpg");
            postParameters.Add("file", new FileUploader.FileParameter(data, "Image.jpg", "image/jpeg"));

            // Create request and receive response
            try
            {
                HttpWebResponse webResponse = FileUploader.MultipartFormDataPost(_serviceUrl, "Prometheus", postParameters);

                // Process response
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                webResponse.Close();

                return JsonConvert.DeserializeObject<ImageDetectionResult>(fullResponse);
            }
            catch (SocketException ex)
            {
                ServiceRunning = false;
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            _worker.RunWorkerAsync();
        }
    }
}
