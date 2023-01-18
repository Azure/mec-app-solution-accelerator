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

        public Alert(string id, Source source, string type, string information, string urlImageEnconded, int accuracy, DateTime captureTime)
        {
            Id = id;
            Source = source;
            Type = type;
            Information = information;
            Frame = urlImageEnconded;
            Accuracy = accuracy;
            CaptureTime = captureTime;
        }

        public string toString()
        {
            return "Alert: " + Id + "\n SourceId: " + Source.Name + "\n Type: " + Type + "\n Information: " + Information + "\n Image: " + Frame + "\n Accuracy: " + Accuracy + "\n Initial time: " + CaptureTime;
        }
    }
}
