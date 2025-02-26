using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NeuroTrade.Models;
using Newtonsoft.Json.Linq;

namespace NeuroTrade.Services
{
    public class YahooFinanceService
    {
        private readonly HttpClient _httpClient;

        private DateTime dateTime;
        private decimal open;
        private decimal high;
        private decimal low;
        private decimal close;
        private long volume;

        public event Action<Stock>? OnStockDataRecieved;


        public YahooFinanceService()
        {
            _httpClient = new HttpClient();
        }

        public async Task GetStockDataAsync(string symbol)
        {
            string url = $"https://stooq.com/q/l/?s={symbol}&f=sd2t2ohlcv&h&e=csv";

            using var client = new HttpClient();
            string csv = await client.GetStringAsync(url);

            Debug.WriteLine("Raw CSV Data:\n" + csv);

            // Parse CSV Data
            var lines = csv.Split('\n');
            if (lines.Length > 1)
            {
                try
                {
                    var data = lines[1].Split(',');

                    if (data[3] == "N/D" || data[4] == "N/D" || data[5] == "N/D" || data[6] == "N/D" || data[7] == "N/D")
                    {
                        Console.WriteLine("Error: Data contains 'N/D'");
                    }

                    open = decimal.Parse(data[3], CultureInfo.InvariantCulture);
                    high = decimal.Parse(data[4], CultureInfo.InvariantCulture);
                    low = decimal.Parse(data[5], CultureInfo.InvariantCulture);
                    close = decimal.Parse(data[6], CultureInfo.InvariantCulture);
                    volume = long.Parse(data[7], CultureInfo.InvariantCulture);

                    var stock = new Stock
                    {
                        Symbol = symbol,
                        CurrentPrice = close,
                        Close = close,
                        Open = open,
                        High = high,
                        Low = low,
                        Volume = volume,
                        LastUpdated = DateTime.Now
                    };

                    OnStockDataRecieved?.Invoke(stock);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine("Error: No data found.");
            }
        }
    }
}
