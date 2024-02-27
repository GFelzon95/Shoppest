namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductCategoryRepository ProductCategories { get; }
        IProductRepository Products { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IApplicationUserRepository ApplicationUsers { get; }

        void Save();
    }
}
