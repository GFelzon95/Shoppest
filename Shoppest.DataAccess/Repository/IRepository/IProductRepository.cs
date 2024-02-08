using Shoppest.Models;

namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);

    }
}
