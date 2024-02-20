using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoppest.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public int ProductCategoryId { get; set; }

        [ForeignKey("ProductCategoryId")]
        [ValidateNever]
        public ProductCategory ProductCategory { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public int Quantity { get; set; }
        [ValidateNever]
        public string Description { get; set; }

        [MaxLength(100)]
        [ValidateNever]
        public string PictureUrl { get; set; }
    }
}
