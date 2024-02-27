using Shoppest.Models;

namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart Cart);
    }
}
