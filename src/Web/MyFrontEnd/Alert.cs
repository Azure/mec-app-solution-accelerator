using System.ComponentModel.DataAnnotations.Schema;

namespace MyFrontEnd
{
    [Table("Alert")]
    public class Alert
    {
        public string Information { get; set; }
        public int Accuracy { get; set; }
        public string UrlVideoEncoded { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        public double lat { get; set; }
        public double @long { get; set; }
        public DateTime AlertTriggerTimeFin { get; set; }
    }
}
