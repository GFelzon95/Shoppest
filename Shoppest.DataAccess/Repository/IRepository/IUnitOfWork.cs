﻿namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductCategoryRepository ProductCategories { get; }
        IProductRepository Products { get; }
        IShoppingCartRepository ShoppingCarts { get; }

        void Save();
    }
}
