using System;

namespace NeuroTrade.Models
{
    public class UserTransaction
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public required User User { get; set; }

        public int StockId { get; set; }
        public required Stock Stock { get; set; }

        public int Shares { get; set; }
        public decimal PricePerShare { get; set; }

        public DateTime TransactionDate { get; set; }
        public bool IsBuy { get; set; } // True for Buy, False for Sell
    }
}
