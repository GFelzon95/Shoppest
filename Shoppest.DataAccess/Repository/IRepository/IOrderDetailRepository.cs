using Shoppest.Models;

namespace Shoppest.DataAccess.Repository.IRepository
{
	public interface IOrderDetailRepository : IRepository<OrderDetail>
	{
		void Update(OrderDetail orderDetail);
	}
}
