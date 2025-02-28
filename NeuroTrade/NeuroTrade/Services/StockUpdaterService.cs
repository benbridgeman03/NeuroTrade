using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeuroTrade.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using NeuroTrade.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace NeuroTrade.Services
{
    public class StockUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly YahooFinanceService _yahooFinanceService;
        private List<string> _watchlist;
        private readonly string filePath = "D:\\Projects\\Other\\Software\\New\\NeuroTrade\\NeuroTrade\\NeuroTrade\\Data\\Watchlist.txt";

        public StockUpdaterService(IServiceProvider services, YahooFinanceService yahooFinanceService)
        {
            _services = services;
            _yahooFinanceService = yahooFinanceService;

            _yahooFinanceService.OnStockDataReceived += OnStockDataReceived;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("StockUpdaterService has started!");

            while (!stoppingToken.IsCancellationRequested)
            {
                _watchlist = ParseWatchList();

                foreach(var element in _watchlist)
                {
                    Debug.WriteLine($"Stock {element} added to watchlist.");
                }

                foreach (var symbol in _watchlist)
                {
                    _ = _yahooFinanceService.GetStockDataAsync(symbol);
                    await Task.Delay(500);
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
                    dbContext.Stocks.Add(stock);
                }
                else
                {
                    existingStock.CurrentPrice = stock.CurrentPrice;
                    existingStock.Open = stock.Open;
                    existingStock.High = stock.High;
                    existingStock.Low = stock.Low;
                    existingStock.Volume = stock.Volume;
                    existingStock.LastUpdated = stock.LastUpdated;

                    dbContext.Stocks.Update(existingStock);
                }

                var intervalStart = DateTime.UtcNow.AddMinutes(-(DateTime.UtcNow.Minute % 5))
                                               .AddSeconds(-DateTime.UtcNow.Second)
                                               .AddMilliseconds(-DateTime.UtcNow.Millisecond);

                var existingOHLC = await dbContext.StockPriceHistories
                    .FirstOrDefaultAsync(h => h.StockId == existingStock.Id && h.IntervalStart == intervalStart);

                if (existingOHLC == null)
                {
                    var newOHLC = new StockPriceHistory
                    {
                        StockId = existingStock.Id,
                        Stock = existingStock,
                        Open = stock.Open,
                        High = stock.High,
                        Low = stock.Low,
                        Close = stock.CurrentPrice,
                        Volume = stock.Volume,
                        IntervalStart = intervalStart
                    };

                    dbContext.StockPriceHistories.Add(newOHLC);
                }
                else
                {
                    existingOHLC.High = Math.Max(existingOHLC.High, stock.High);
                    existingOHLC.Low = Math.Min(existingOHLC.Low, stock.Low);
                    existingOHLC.Close = stock.CurrentPrice;
                    existingOHLC.Volume += stock.Volume;

                    dbContext.StockPriceHistories.Update(existingOHLC);
                }

                await dbContext.SaveChangesAsync();
                Debug.WriteLine($"Stock {stock.Symbol} updated in the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database Error: {ex.Message}");
            }
        }


        private List<string> ParseWatchList()
        {
            string symbols = File.ReadAllText(filePath);
            List<string> stockSymbols = new List<string>(symbols.Split(", ", StringSplitOptions.RemoveEmptyEntries));
            return stockSymbols;
        }
    }
}
