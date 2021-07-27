using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler: IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;
        
        public UpdateOrderCommandHandler(ILogger<UpdateOrderCommandHandler> logger, IMapper mapper, IOrderRepository orderRepository)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }
        
        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await this._orderRepository.GetByIdAsync(request.Id);

            if (orderToUpdate is null)
            {
                this._logger.LogError("Order isn't exist on database");
                throw new NotFoundException(nameof(UpdateOrderCommand), typeof(Order));
            }
            
            // maps from request dto to order entity new fields
            this._mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));
            
            await this._orderRepository.UpdateAsync(orderToUpdate);
            
            this._logger.LogInformation($"Order with {orderToUpdate.Id} Id is successfully updated.");
            
            // empty value by mediatr
            return Unit.Value; 
        }
    }
}