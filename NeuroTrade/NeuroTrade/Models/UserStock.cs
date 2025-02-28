namespace NeuroTrade.Models
{
    public class UserStock
    {
        public int Id { get; set; }

        public int StockId { get; set; }
        public required Stock Stock { get; set; }

        public int UserId { get; set; }
        public required User User { get; set; }

        public int SharesOwned { get; set; }
        public decimal AveragePurchasePrice { get; set; }

        public decimal CurrentValue => SharesOwned * Stock.CurrentPrice;
    }
}
