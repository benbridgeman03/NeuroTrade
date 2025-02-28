using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeuroTrade.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using NeuroTrade.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace NeuroTrade.Services
{
    public class StockUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly YahooFinanceService _yahooFinanceService;
        private readonly string[] _watchlist = { "AAPL" };

        public StockUpdaterService(IServiceProvider services, YahooFinanceService yahooFinanceService)
        {
            _services = services;
            _yahooFinanceService = yahooFinanceService;

            _yahooFinanceService.OnStockDataRecieved += OnStockDataReceived;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("StockUpdaterService has started!");

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var symbol in _watchlist)
                {
                    _ = _yahooFinanceService.GetStockDataAsync(symbol);
                }

                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }
        }

        private async void OnStockDataReceived(Stock stock)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                var existingStock = await dbContext.Stocks.FirstOrDefaultAsync(s => s.Symbol == stock.Symbol);
                if (existingStock == null)
                {
                    dbContext.Add(stock);
                }
                else
                {
                    existingStock.CurrentPrice = stock.CurrentPrice;
                    existingStock.Open = stock.Open;
                    existingStock.High = stock.High;
                    existingStock.Low = stock.Low;
                    existingStock.Volume = stock.Volume;
                    existingStock.LastUpdated = stock.LastUpdated;

                    dbContext.Update(existingStock);
                }

                await dbContext.SaveChangesAsync();
                Debug.WriteLine($"Stock {stock.Symbol} updated in the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }
        }
    }
}
