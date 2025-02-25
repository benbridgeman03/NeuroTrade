using System;

namespace NeuroTrade.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal CurrentPrice { get; set; } 
        public decimal Open { get; set; }
        public decimal Close{ get; set; }
        public decimal High{ get; set; }
        public decimal Low{ get; set; }
        public long Volume { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
