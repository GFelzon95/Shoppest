using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;

namespace Shoppest.DataAccess.Repository
{
    public class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ProductCategory productCategory)
        {
            _context.Update(productCategory);
        }
    }
}
