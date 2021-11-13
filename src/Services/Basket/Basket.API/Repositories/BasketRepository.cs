using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            this._redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }


        public async Task<ShoppingCart> GetBasketAsync(string userName)
        {
            var basket = await this._redisCache.GetStringAsync(userName);

            if (string.IsNullOrWhiteSpace(basket)) return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket)
        {
            await this._redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasketAsync(basket.UserName);
        }

        public async Task DeleteBasketAsync(string userName)
        {
            await this._redisCache.RemoveAsync(userName);
        }
    }
}
