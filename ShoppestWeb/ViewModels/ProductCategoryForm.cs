using ShoppestWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace ShoppestWeb.ViewModels
{
    public class ProductCategoryForm
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public FormSettings? FormSettings { get; set; }
        public bool IsDelete { get; set; }
    }
}
