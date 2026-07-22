using System.ComponentModel.DataAnnotations;

namespace CurrencyManagement.Models
{
    public class Currency
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Currency Code is required.")]
        [StringLength(3, MinimumLength = 3,
            ErrorMessage = "Currency Code must be exactly 3 characters.")]
        [Display(Name = "Currency Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Currency Name is required.")]
        [StringLength(100)]
        [Display(Name = "Currency Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // ===== FX Rate =====

        [Display(Name = "Current Rate")]
        public decimal CurrentRate { get; set; } = 0;

        [Display(Name = "Previous Rate")]
        public decimal PreviousRate { get; set; } = 0;

        [Display(Name = "Change (%)")]
        public decimal ChangePercent { get; set; } = 0;

        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }
    }
}