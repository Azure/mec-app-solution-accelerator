using System.ComponentModel.DataAnnotations.Schema;

namespace MyFrontEnd.Models
{
    public class Alert
    {
        public string Id { get; set; }
        //public string SourceId { get; set; }
        //public string Priority { get; set; } //Quitar
        public Source Source { get; set; }
        public string Type { get; set; }
        public string Information { get; set; }
        public string Frame { get; set; }
        public float Accuracy { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        //public DateTime AlertTriggerTimeFin { get; set; }

        //public Alert(string id, string sourceId, string information, string urlImageEnconded, int accuracy, DateTime alertTriggerTimeIni)
        //{
        //    Id = id;
        //    IdName = sourceId;
        //    Information = information;
        //    Frame = urlImageEnconded;
        //    Accuracy = accuracy;
        //    AlertTriggerTimeIni = alertTriggerTimeIni;
        //}

        //public string toString()
        //{
        //    return "Alert: " + Id + "\n SourceId: " + SourceId + "\n Priority: " + Priority + "\n Information: " + Information + "\n Image: " + UrlImageEncoded + "\n Accuracy: " + Accuracy + "\n Initial time: " + AlertTriggerTimeIni;
        //}
    }
}
