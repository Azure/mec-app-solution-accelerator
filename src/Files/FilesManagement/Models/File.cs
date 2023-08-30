using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.MecSolutionAccelerator.Services.Files.Models
{
    [Table("Alert")]
    public class Alert : AEntity
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
    }
}
