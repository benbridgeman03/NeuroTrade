using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using NeuroTrade.Models;
using Newtonsoft.Json.Linq;

namespace NeuroTrade.Services
{
    public class YahooFinanceService
    {
        private readonly HttpClient _httpClient;

        public YahooFinanceService()
        {
            _httpClient = new HttpClient();
        }


        public async Task<Stock?> GetStockDataAsync(string symbol)
        {
            string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{symbol}?interval=1d&range=1mo";

            try
            {
                Debug.WriteLine($"Requesting Yahoo Finance API for {symbol}...");

                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Debug.WriteLine($"Yahoo Finance rate limit hit for {symbol}. Retrying in 60 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(60));
                    return null;  
                }

                response.EnsureSuccessStatusCode();

                var json = JObject.Parse(await response.Content.ReadAsStringAsync());

                var result = json["chart"]?["result"]?.First;
                if (result == null)
                {
                    Debug.WriteLine($"Invalid response for {symbol}. Skipping...");
                    return null;
                }

                var meta = result["meta"];
                var indicators = result["indicators"]["quote"].First;

                return new Stock
                {
                    Symbol = symbol,
                    CurrentPrice = meta["regularMarketPrice"]?.Value<decimal>() ?? 0,
                    Open = indicators["open"].Last?.Value<decimal>() ?? 0,
                    High = indicators["high"].Last?.Value<decimal>() ?? 0,
                    Low = indicators["low"].Last?.Value<decimal>() ?? 0,
                    Close = indicators["close"].Last?.Value<decimal>() ?? 0,
                    Volume = indicators["volume"].Last?.Value<long>() ?? 0
                };
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Network error while fetching {symbol}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error for {symbol}: {ex.Message}");
                return null;
            }
        }
    }
}
