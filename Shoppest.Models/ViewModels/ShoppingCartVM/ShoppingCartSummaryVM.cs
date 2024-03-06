namespace Shoppest.Models.ViewModels.ShoppingCartVM
{
    public class ShoppingCartSummaryVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
