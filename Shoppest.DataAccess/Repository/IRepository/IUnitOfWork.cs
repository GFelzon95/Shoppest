namespace Shoppest.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		IProductCategoryRepository ProductCategories { get; }
		IProductRepository Products { get; }
		IShoppingCartRepository ShoppingCarts { get; }
		IApplicationUserRepository ApplicationUsers { get; }
		IOrderDetailRepository OrderDetails { get; }
		IOrderHeaderRepository OrderHeaders { get; }

		void Save();
	}
}
