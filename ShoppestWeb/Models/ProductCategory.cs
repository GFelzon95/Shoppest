using System.ComponentModel.DataAnnotations;

namespace ShoppestWeb.Models
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
