using System;
using System.Threading.Tasks;
using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository: IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        
        public DiscountRepository(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public async Task<Coupon> GetDiscountAsync(string productName)
        {
            using var connection = new NpgsqlConnection(
                this._configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            // using query like this by Dapper
            var coupon = await connection.QueryFirstOrDefaultAsync(
                "SELECT * FROM Coupon WHERE ProductName = @ProductName", 
                new { ProductName = productName });
            
            if (coupon is null) 
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount" };

            return coupon;
        }

        public async Task<bool> CreateDiscountAsync(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(
                this._configuration.GetValue<string>("DataSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync(
                @"INSERT INTO Coupon (ProductName, Description, Amount) 
                        VALUES (@ProductName, @Description, @Amount)",
                new
                {
                    ProductName = coupon.ProductName,
                    Description = coupon.Description,
                    Amount = coupon.Amount
                });

            if (affected == 0) 
                return false;

            return true;
        }

        public async Task<bool> UpdateDiscountAsync(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(
                this._configuration.GetValue<string>("DataSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync(
                @"UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
                new 
                { 
                    ProductName = coupon.ProductName, 
                    Description = coupon.Description, 
                    Amount = coupon.Amount, 
                    Id = coupon.Id 
                });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscountAsync(string productName)
        {
            using var connection = new NpgsqlConnection(
                this._configuration.GetValue<string>("DataSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync(
                "DELETE FROM Coupon WHERE ProductName=@ProductName",
                new { ProductName = productName });
            
            if (affected == 0)
                return false;
            
            return true;
        }
    }
}