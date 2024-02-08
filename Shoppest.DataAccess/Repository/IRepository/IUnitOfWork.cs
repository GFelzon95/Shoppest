namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductCategoryRepository ProductCategory { get; }

        void Save();
    }
}
