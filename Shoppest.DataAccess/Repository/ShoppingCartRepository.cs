using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;

namespace Shoppest.DataAccess.Repository
{
    internal class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(ShoppingCart Cart)
        {
            _context.Update(Cart);
        }
    }
}
