using System.ComponentModel.DataAnnotations.Schema;

namespace MyFrontEnd.Models
{
    public class Alert
    {
        public string Id { get; set; }
        public Source Source { get; set; }
        public string Type { get; set; }
        public string? Information { get; set; }
        public string? Frame { get; set; }
        public float Accuracy { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime AlertTime { get; set; }
        public double MsExecutionTime { get; set; }

        public Alert(string id, Source source, string type, string? information, string? frame, float accuracy, DateTime captureTime, DateTime alertTime, double msExecutionTime)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Information = information;
            Frame = frame;
            Accuracy = accuracy;
            CaptureTime = captureTime;
            AlertTime = alertTime;
            MsExecutionTime = msExecutionTime;
        }

        public string toString()
        {
            return "Alert: " + Id + "\n SourceId: " + Source.Name + "\n Type: " + Type + "\n Information: " + Information + "\n Image: " + Frame + "\n Accuracy: " + Accuracy + "\n Initial time: " + CaptureTime;
        }
    }
}
