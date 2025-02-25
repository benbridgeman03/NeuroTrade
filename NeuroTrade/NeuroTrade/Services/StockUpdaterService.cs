using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeuroTrade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeuroTrade.Models;
using System.Diagnostics;

namespace NeuroTrade.Services
{
    public class StockUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly YahooFinanceService _yahooFinanceService;
        private readonly string[] _watchlist = { "AAPL"};

        public StockUpdaterService(IServiceProvider Services, YahooFinanceService yahooFinanceService)
        {
            _services = Services;
            _yahooFinanceService = yahooFinanceService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("StockUpdaterService has started!");

            while (!stoppingToken.IsCancellationRequested)
            {

                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    foreach (var symbol in _watchlist)
                    {
                        try
                        {
                            var stockData = await _yahooFinanceService.GetStockDataAsync(symbol);

                            if (stockData == null)
                            {
                                Console.WriteLine($" Skipping {symbol} update due to API limit.");
                                continue; 
                            }

                            var stock = await dbContext.Stocks.FindAsync(symbol);

                            if (stock == null)
                            {
                                stock = new Stock
                                {
                                    Symbol = symbol,
                                    LastUpdated = DateTime.UtcNow
                                };
                                dbContext.Add(stock);
                            }

                            stock.CurrentPrice = stockData.CurrentPrice;
                            stock.Open = stockData.Open;
                            stock.High = stockData.High;
                            stock.Low = stockData.Low;
                            stock.Close = stockData.Close;
                            stock.Volume = stockData.Volume;
                            stock.LastUpdated = DateTime.UtcNow;

                            await dbContext.SaveChangesAsync();

                            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating {symbol}: {ex.Message}");
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }
        }

    }
}
