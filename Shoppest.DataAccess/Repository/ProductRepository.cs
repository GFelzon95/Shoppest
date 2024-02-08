using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;

namespace Shoppest.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            _context.Update(product);
        }
    }
}
