using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using NeuroTrade.Models;
using YahooFinanceApi;

namespace NeuroTrade.Services
{
    public class YahooFinanceService
    {
        private readonly HttpClient _httpClient;
        public event Action<Stock>? OnStockDataReceived;

        public YahooFinanceService()
        {
            _httpClient = new HttpClient();
        }

        public async Task GetStockDataAsync(string symbol)
        {
            try
            {
                var securities = await Yahoo.Symbols(symbol)
                    .Fields(
                        Field.RegularMarketPrice,
                        Field.RegularMarketOpen,
                        Field.RegularMarketPreviousClose,
                        Field.RegularMarketVolume,
                        Field.RegularMarketDayHigh,
                        Field.RegularMarketDayLow,
                        Field.MarketState
                    )
                    .QueryAsync();

                var stockData = securities[symbol];

                string marketState = SafeGetString(stockData, Field.MarketState, "CLOSED");
                bool isMarketOpen = marketState == "REGULAR";

                var stock = new Stock
                {
                    Symbol = symbol,
                    CurrentPrice = SafeGetDecimal(stockData, Field.RegularMarketPrice),
                    Close = SafeGetDecimal(stockData, Field.RegularMarketPreviousClose),
                    Open = SafeGetDecimal(stockData, Field.RegularMarketOpen),
                    Volume = SafeGetLong(stockData, Field.RegularMarketVolume),
                    LastUpdated = DateTime.Now
                };

                if (isMarketOpen)
                {
                    stock.High = SafeGetDecimal(stockData, Field.RegularMarketDayHigh, stock.High);
                    stock.Low = SafeGetDecimal(stockData, Field.RegularMarketDayLow, stock.Low);
                }

                Debug.WriteLine($@"
                    Stock Data:
                    ------------
                    Symbol        : {stock.Symbol}
                    Current Price : {stock.CurrentPrice}
                    Close Price   : {stock.Close}
                    Open Price    : {stock.Open}
                    High Price    : {(isMarketOpen ? stock.High.ToString() : "Unchanged")}
                    Low Price     : {(isMarketOpen ? stock.Low.ToString() : "Unchanged")}
                    Volume        : {stock.Volume}
                    Market State  : {marketState}
                    Last Updated  : {stock.LastUpdated}
                    ");

                OnStockDataReceived?.Invoke(stock);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting stock data for {symbol}: {ex.Message}");
            }
        }


        private decimal SafeGetDecimal(Security stockData, Field field, decimal defaultValue = 0)
        {
            try
            {
                return stockData[field] != null ? Convert.ToDecimal(stockData[field]) : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private long SafeGetLong(Security stockData, Field field, long defaultValue = 0)
        {
            try
            {
                return stockData[field] != null ? Convert.ToInt64(stockData[field]) : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        private string SafeGetString(Security stockData, Field field, string defaultValue = "")
        {
            try
            {
                return stockData[field]?.ToString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
