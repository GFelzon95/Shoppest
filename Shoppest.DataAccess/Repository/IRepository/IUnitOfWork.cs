namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductCategoryRepository ProductCategories { get; }

        void Save();
    }
}
