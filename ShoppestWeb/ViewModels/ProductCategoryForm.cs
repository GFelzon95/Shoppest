using ShoppestWeb.Models;

namespace ShoppestWeb.ViewModels
{
    public class ProductCategoryForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FormSettings? FormSettings { get; set; }
    }
}
