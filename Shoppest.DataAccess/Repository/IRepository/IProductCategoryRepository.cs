using Shoppest.Models;

namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IProductCategoryRepository : IRepository<ProductCategory>
    {
        void Update(ProductCategory productCategory);
    }
}
