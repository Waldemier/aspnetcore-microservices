using System;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    /// <summary>
    /// Grpc server side class
    /// </summary>
    public class DiscountService: DiscountProtoService.DiscountProtoServiceBase // class that generated from Protos/discount.proto file
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;
        
        public DiscountService(ILogger<DiscountService> logger, IDiscountRepository repository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await this._repository.GetDiscountAsync(request.ProductName);

            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with ProductName={request.ProductName} is not found."));
            }

            this._logger.LogInformation("Discount is retrieved for ProductName: {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);
            
            var couponModel = this._mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = this._mapper.Map<Coupon>(request.Coupon); //CouponModel type to coupon entity

            await this._repository.CreateDiscountAsync(coupon);
            this._logger.LogInformation("Discount is successfully created. ProductName: {ProductName}", coupon.ProductName);

            var couponModel = this._mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = this._mapper.Map<Coupon>(request.Coupon); //CouponModel type to coupon entity

            await this._repository.UpdateDiscountAsync(coupon);
            this._logger.LogInformation("Discount is successfully updated. ProductName: {ProductName}", coupon.ProductName);

            var couponModel = this._mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            bool deleted = await this._repository.DeleteDiscountAsync(request.ProductName);
            
            this._logger.LogInformation("Discount is successfully deleted. ProductName: {ProductName}", request.ProductName);
            
            return new DeleteDiscountResponse() { Success = deleted };
        }
    }
}