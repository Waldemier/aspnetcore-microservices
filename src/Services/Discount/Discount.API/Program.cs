<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ordering.API
=======
using Discount.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Discount.API
>>>>>>> 780b3e92c670c01094b03b3449eef6f9d1d4b82a
{
    public class Program
    {
        public static void Main(string[] args)
        {
<<<<<<< HEAD
            CreateHostBuilder(args).Build().Run();
=======
            var host = CreateHostBuilder(args).Build();
            host.MigrateDatabase<Program>(); // Передаємо тип Program в generic для ILogger логування.
            host.Run();
>>>>>>> 780b3e92c670c01094b03b3449eef6f9d1d4b82a
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}