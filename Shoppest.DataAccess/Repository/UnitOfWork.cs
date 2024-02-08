using Shoppest.DataAccess.Repository.IRepository;

namespace Shoppest.DataAccess.Repository
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IProductCategoryRepository ProductCategory { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ProductCategory = new ProductCategoryRepository(_context);

        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
