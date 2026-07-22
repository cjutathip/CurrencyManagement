using System.ComponentModel.DataAnnotations;

namespace CurrencyManagement.Models
{
    public class ConvertViewModel
    {
        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "From Currency")]
        public string FromCurrency { get; set; } = "USD";

        [Required]
        [Display(Name = "To Currency")]
        public string ToCurrency { get; set; } = "THB";

        public decimal Result { get; set; }

        public decimal ExchangeRate { get; set; }
    }
}