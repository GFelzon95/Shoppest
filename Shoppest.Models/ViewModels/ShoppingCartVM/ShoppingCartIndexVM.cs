namespace Shoppest.Models.ViewModels.ShoppingCartVM
{
    public class ShoppingCartIndexVM
    {
        public IEnumerable<ShoppingCart>? ShoppingCartList { get; set; }
        public double TotalPrice { get; set; }
    }
}
