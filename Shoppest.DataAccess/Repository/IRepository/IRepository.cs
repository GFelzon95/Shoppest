using System.Linq.Expressions;

namespace Shoppest.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Get(Expression<Func<T, bool>> filter, string? Properties = null);
        IEnumerable<T> GetAll(string? Properties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);


    }
}
