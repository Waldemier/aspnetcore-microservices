using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersListQueryHandler: IRequestHandler<GetOrdersListQuery, List<OrdersVm>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrdersListQueryHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<List<OrdersVm>> Handle(GetOrdersListQuery request, 
            CancellationToken cancellationToken)
        {
            var orderList = await this._orderRepository.GetOrdersByUserNameAsync(request.UserName);
            return this._mapper.Map<List<OrdersVm>>(orderList);
        }
    }
}