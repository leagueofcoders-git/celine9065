using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace urlShortener.DataModel
{
    [Table("urlShortener", Schema = "dbo")]
    public class shortener
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int URLId { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        [Required]
        public string longURL { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string shortURL { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Required]
        public string URLparam { get; set; }

        public users? userId { get; set; }
    }
}
