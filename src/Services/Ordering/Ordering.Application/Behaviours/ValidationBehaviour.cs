using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

// вказання псевдоніму у звязку з тим, що такий клас уже існує в просторі імен FluentValidation'а
using ValidationException =  Ordering.Application.Exceptions.ValidationException;

namespace Ordering.Application.Behaviours
{
    /// <summary>
    /// Mediatr pre-pipeline behaviour (middleware),
    /// that checks whether fluent validator has any validation errors.
    /// </summary>
    /// <typeparam name="TRequest"> Can be UpdateOrderCommand / CheckoutOrderCommand / DeleteOrderCommand </typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehaviour<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    {
        // Interface from FluentValidation
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            this._validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, 
            RequestHandlerDelegate<TResponse> next)
        {
            if (this._validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults =
                    await Task.WhenAll(this._validators.Select(validator => 
                        validator.ValidateAsync(context, cancellationToken)));
                
                var failures = validationResults.SelectMany(result => result.Errors)
                    .Where(failure => failure != null)
                    .ToList();

                if (failures.Count != 0) throw new ValidationException(failures); // Custom validation exception
            }

            return await next();
        }
    }
}