using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Behaviours
{
    /// <summary>
    /// Additional mediatr behaviour exception middleware,
    /// that catches unhandled (unexpected) exceptions from commands execution code (commands handle method). 
    /// </summary>
    /// <typeparam name="TRequest"> Can be UpdateOrderCommand / CheckoutOrderCommand / DeleteOrderCommand </typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class UnhandledExceptionBehaviour<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                // try to execute mediatr command functionality (UpdateOrderCommand, etc.) after validation behaviour middleware. 
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                this._logger.LogError(ex, "Application Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
                throw;
            }
        }
    }
}