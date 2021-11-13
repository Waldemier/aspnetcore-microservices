using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Basket.API.GrpcServices;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;
        
        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        }


        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasketAsync(string userName) =>
            Ok(await this._repository.GetBasketAsync(userName) ?? new ShoppingCart(userName));


        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasketAsync([FromBody] ShoppingCart basket)
        {
            foreach (var item in basket.Items)
            {
                var coupon = await this._discountGrpcService.GetDiscountAsync(item.ProductName);
                item.Price -= coupon.Amount;
            }
            return Ok(await this._repository.UpdateBasketAsync(basket));
        }


        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasketAsync(string userName)
        {
            await this._repository.DeleteBasketAsync(userName);
            return Ok();
        }
    }
}
