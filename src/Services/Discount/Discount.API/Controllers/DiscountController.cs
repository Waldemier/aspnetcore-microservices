using System;
using System.Net;
using System.Threading.Tasks;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController: ControllerBase
    {
        private readonly IDiscountRepository _repository;
        
        public DiscountController(IDiscountRepository repository)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> GetCouponAsync(string productName) => 
            Ok(await this._repository.GetDiscountAsync(productName));

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> CreateDiscountAsync([FromBody] Coupon coupon)
        {
            await this._repository.CreateDiscountAsync(coupon);
            return CreatedAtRoute("GetDiscount", new {productName = coupon.ProductName}, coupon);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> UpdateDiscountAsync([FromBody] Coupon coupon) =>
            Ok(await this._repository.UpdateDiscountAsync(coupon));

        [HttpDelete("{productName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscountAsync(string productName) =>
            Ok(await this._repository.DeleteDiscountAsync(productName));
    }
}