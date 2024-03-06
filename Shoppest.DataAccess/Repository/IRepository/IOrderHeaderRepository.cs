using Shoppest.Models;

namespace Shoppest.DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);
	}
}
