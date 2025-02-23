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
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>();

            return services.BuildServiceProvider();
        }
    }
}
