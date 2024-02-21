using System.ComponentModel.DataAnnotations;

namespace Shoppest.Models.ViewModels.ProductCategoryVM
{
    public class ProductCategoryFormVM
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public FormSettings? FormSettings { get; set; }
        public bool IsDelete { get; set; }
    }
}
