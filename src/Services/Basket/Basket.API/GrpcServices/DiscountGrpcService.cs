using System;
using System.Threading.Tasks;
using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices
{
    /// <summary>
    /// Grpc client side class
    /// </summary>
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;
        
        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            this._discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<CouponModel> GetDiscountAsync(string productName)
        {
            var discountRequest = new GetDiscountRequest() { ProductName = productName };

            return await this._discountProtoService.GetDiscountAsync(discountRequest);
        } 
    }
}