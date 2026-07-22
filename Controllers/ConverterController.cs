using Microsoft.AspNetCore.Mvc;
using CurrencyManagement.Models;
using CurrencyManagement.Services;

namespace CurrencyManagement.Controllers
{
    public class ConverterController : Controller
    {
        private readonly FxRateService _fxRateService;

        public ConverterController(FxRateService fxRateService)
        {
            _fxRateService = fxRateService;
        }

        // ==========================
        // GET
        // ==========================
        [HttpGet]
        public IActionResult Index()
        {
            ConvertViewModel model = new ConvertViewModel();

            model.Amount = 1;
            model.FromCurrency = "USD";
            model.ToCurrency = "THB";

            return View(model);
        }

        // ==========================
        // POST
        // ==========================
        [HttpPost]
        public async Task<IActionResult> Index(ConvertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string fromCurrency =
                    model.FromCurrency.Trim().ToUpper();

                string toCurrency =
                    model.ToCurrency.Trim().ToUpper();

                // ถ้าเลือกสกุลเงินเดียวกัน
                if (fromCurrency == toCurrency)
                {
                    model.ExchangeRate = 1.0000m;
                    model.Result = model.Amount;

                    return View(model);
                }

                FrankfurterResponse? response =
                    await _fxRateService.GetRate(
                        fromCurrency,
                        toCurrency);

                if (response == null)
                {
                    ViewBag.Error = "Cannot get exchange rate.";
                    return View(model);
                }

                if (!response.Rates.ContainsKey(toCurrency))
                {
                    ViewBag.Error = "Currency not found.";
                    return View(model);
                }

                decimal rate =
                    response.Rates[toCurrency];

                model.ExchangeRate = rate;
                model.Result = model.Amount * rate;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
    }
}