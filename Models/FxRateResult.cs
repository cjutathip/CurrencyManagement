namespace CurrencyManagement.Models
{
    public class FxRateResult
    {
        public decimal CurrentRate { get; set; }

        public decimal PreviousRate { get; set; }

        public decimal ChangePercent { get; set; }

        public DateTime RateDate { get; set; }
    }
}