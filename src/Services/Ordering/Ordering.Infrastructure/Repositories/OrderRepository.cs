using System.Collections.Generic;
using System.Threading.Tasks;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository: RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext orderContext) : base(orderContext)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName)
        {
            return await GetAsync(x => x.UserName.Equals(userName));
        }
    }
}