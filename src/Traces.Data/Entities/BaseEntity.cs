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
        public Instant UpdatedUtc { get; set; }

        /// <summary>
        /// This refers to the account_code from the JWT
        /// </summary>
        [Required]
        public string TenantId { get; set; }
    }
}