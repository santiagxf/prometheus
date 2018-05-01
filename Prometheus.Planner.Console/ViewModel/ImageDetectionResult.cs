using System.Collections.Generic;

namespace Prometeo.Planner.Console
{
    public class ImageDetectionResult
    {
        public ImageDetectionResult()
        {
            labels = new List<int>();
            scores = new List<double>();
            boxes = new List<List<double>>();
        }
        public string fileName { get; set; }
        public string scoringId { get; set; }
        public string executionTimeMs { get; set; }
        public List<int> labels { get; set; }
        public List<double> scores { get; set; }
        public List<List<double>> boxes { get; set; }
    }
}