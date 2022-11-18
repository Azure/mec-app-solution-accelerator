using System.ComponentModel.DataAnnotations;

namespace Microsoft.MecSolutionAccelerator.Services.Commons
{
    public abstract class AEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
