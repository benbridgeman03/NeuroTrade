﻿using System;

namespace NeuroTrade.Models
{
    public class StockPriceHistory
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public required Stock Stock { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }

        public DateTime IntervalStart { get; set; }
    }

}
