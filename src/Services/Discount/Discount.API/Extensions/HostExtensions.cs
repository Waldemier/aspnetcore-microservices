using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        /// <summary>
        /// Configure and migrating data while assembly executing.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="retry">The number of attempts after unsuccessful connections.</param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns>Host</returns>
        public static IHost MigrateDatabase<TContext>(this IHost host, int retry = 0) where TContext: Program
        {
            int retryForAvailability = retry; 

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgresql database.");
                    
                    using var connection = new NpgsqlConnection(
                        configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    
                    // Відкриваємо connection для подальшого впровадження команд, які реалізовані нижче.
                    // Без цього викине exception.
                    connection.Open();

                    var command = new NpgsqlCommand()
                    {
                        Connection = connection
                    };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon;";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(
                                                Id SERIAL PRIMARY KEY,
                                                ProductName VARCHAR(24) NOT NULL,
                                                Description TEXT,
                                                Amount INT
                                          );";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        "INSERT INTO Coupon(ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone X discount', 150);";
                    command.ExecuteNonQuery();
                    
                    command.CommandText =
                        "INSERT INTO Coupon(ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung 10 discount', 100);";
                    command.ExecuteNonQuery(); 
                    
                    logger.LogInformation("PostgreSQL database was migrated.");
                }
                catch (NpgsqlException ex)
                {
                    // If we get connect exception this code below does make the recursion,
                    // which does try to reconnect us to database.
                    
                    logger.LogError(ex, "An error occurred while migrating the postgresql database.");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}