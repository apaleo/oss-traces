using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Traces.Data.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Instant CreatedUtc { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public Instant UpdatedUtc { get; set; }

        [Required]
        public string UpdatedBy { get; set; }

        [Required]
        public string TenantId { get; set; }
    }
}