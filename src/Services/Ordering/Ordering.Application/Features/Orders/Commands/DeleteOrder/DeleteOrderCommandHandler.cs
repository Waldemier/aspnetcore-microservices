using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler: IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;
        private readonly IMapper _mapper;

        public DeleteOrderCommandHandler(ILogger<DeleteOrderCommandHandler> logger, IMapper mapper, IOrderRepository orderRepository)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await this._orderRepository.GetByIdAsync(request.Id);
            
            if (orderToDelete is null)
            {
                this._logger.LogError("Order isn't exist on database");
                throw new NotFoundException(nameof(DeleteOrderCommand), typeof(Order));
            }
            
            await this._orderRepository.DeleteAsync(orderToDelete);
            
            this._logger.LogInformation($"Order with {orderToDelete.Id} Id is successfully deleted.");
            
            return Unit.Value;
        }
    }
}