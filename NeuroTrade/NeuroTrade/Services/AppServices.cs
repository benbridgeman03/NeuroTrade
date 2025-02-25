using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NeuroTrade.Data;

namespace NeuroTrade.Services
{
    public static class AppServices
    {
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>();
            services.AddHostedService<StockUpdaterService>();
            services.AddSingleton<YahooFinanceService>();

            return services;
        }
    }
}
