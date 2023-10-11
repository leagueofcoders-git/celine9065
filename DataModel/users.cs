using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace urlShortener.DataModel
{
    [Table("user", Schema = "dbo")]
    public class users
    {
        [Key]
        public Guid UserId { get; set; }

        [EmailAddress(ErrorMessage = "Email address is not valid!")]
        [Required]
        public string email { get; set; }

        [StringLength(1000, MinimumLength = 8, ErrorMessage = "Password must be 8 characters or more!")]
        [Required]
        public string password { get; set; }

        public string? apiKey { get; set; }
    }
}
