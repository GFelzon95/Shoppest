using Shoppest.DataAccess.Repository.IRepository;

namespace Shoppest.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IProductCategoryRepository ProductCategories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IShoppingCartRepository ShoppingCarts { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ProductCategories = new ProductCategoryRepository(_context);
            Products = new ProductRepository(_context);
            ShoppingCarts = new ShoppingCartRepository(_context);
            ApplicationUsers = new ApplicationUserRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
