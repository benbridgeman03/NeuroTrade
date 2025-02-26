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
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            services.AddSingleton<YahooFinanceService>();
            services.AddHostedService<StockUpdaterService>();
        }
    }

}
