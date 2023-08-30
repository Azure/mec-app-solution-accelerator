using System.ComponentModel.DataAnnotations;

namespace Microsoft.MecSolutionAccelerator.Services.Files.Models
{
    public abstract class AEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}