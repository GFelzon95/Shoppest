namespace Shoppest.Models.ViewModels.OrderVM
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }

        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
