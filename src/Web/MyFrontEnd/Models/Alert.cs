using System.ComponentModel.DataAnnotations.Schema;

namespace MyFrontEnd.Models
{
    public class Alert
    {
        public string Id { get; set; }
        public string SourceId { get; set; }
        public string Priority { get; set; }
        public string Information { get; set; }
        public string UrlImageEncoded { get; set; }
        public int Accuracy { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime AlertTriggerTimeFin { get; set; }

        public Alert(string id, string sourceId, string priority, string information, string urlImageEnconded, int accuracy, DateTime alertTriggerTimeIni)
        {
            Id = id;
            SourceId = sourceId;
            Priority = priority;
            Information = information;
            UrlImageEncoded = urlImageEnconded;
            Accuracy = accuracy;
            AlertTriggerTimeIni = alertTriggerTimeIni;
        }

        public string toString()
        {
            return "Alert: " + Id + "\n SourceId: " + SourceId + "\n Priority: " + Priority + "\n Information: " + Information + "\n Image: " + UrlImageEncoded + "\n Accuracy: " + Accuracy + "\n Initial time: " + AlertTriggerTimeIni;
        }
    }
}
