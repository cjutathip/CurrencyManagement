using Microsoft.AspNetCore.Mvc;
using CurrencyManagement.Models;
using CurrencyManagement.Services;

namespace CurrencyManagement.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly CurrencyService _service;
        private readonly FxRateService _fxRateService;

        public CurrencyController(
            CurrencyService service,
            FxRateService fxRateService)
        {
            _service = service;
            _fxRateService = fxRateService;
        }

        // ==========================
        // Index
        // ==========================
        public IActionResult Index()
        {
            return View(_service.GetAll());
        }

        // ==========================
        // Create
        // ==========================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Currency currency)
        {
            if (!ModelState.IsValid)
                return View(currency);

            _service.Add(currency);

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // Edit
        // ==========================
        public IActionResult Edit(int id)
        {
            var currency = _service.GetById(id);

            if (currency == null)
                return NotFound();

            return View(currency);
        }

        [HttpPost]
        public IActionResult Edit(Currency currency)
        {
            if (!ModelState.IsValid)
                return View(currency);

            _service.Update(currency);

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // Delete
        // ==========================
        public IActionResult Delete(int id)
        {
            _service.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // Refresh One Currency
        // ==========================
        public async Task<IActionResult> Refresh(int id)
        {
            Currency? currency = _service.GetById(id);

            if (currency == null)
            {
                TempData["Error"] = "Currency not found.";
                return RedirectToAction(nameof(Index));
            }

            //  ถ้าเป็น THB ไม่ต้องเรียก API
            if (currency.Code.Trim().ToUpper() == "THB")
            {
                currency.PreviousRate = currency.CurrentRate;
                currency.CurrentRate = 1.0m;
                currency.ChangePercent = 0;
                currency.LastUpdated = DateTime.Now;

                TempData["Success"] = "THB updated.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                FrankfurterResponse? result =
                    await _fxRateService.GetRate(currency.Code);

                if (result != null &&
                    result.Rates.ContainsKey(currency.Code))
                {
                    currency.PreviousRate = currency.CurrentRate;
                    currency.CurrentRate = result.Rates[currency.Code];
                    currency.LastUpdated = DateTime.Now;

                    if (currency.PreviousRate > 0)
                    {
                        currency.ChangePercent =
                            ((currency.CurrentRate - currency.PreviousRate)
                            / currency.PreviousRate) * 100;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // Refresh All
        // ==========================
        public async Task<IActionResult> RefreshAll()
        {
            var currencies = _service.GetAll();

            foreach (var currency in currencies)
            {
                if (currency.Code.Trim().ToUpper() == "THB")
                {
                    currency.PreviousRate = currency.CurrentRate;
                    currency.CurrentRate = 1.0m;
                    currency.ChangePercent = 0;
                    currency.LastUpdated = DateTime.Now;
                    continue;
                }
                try
                {
                    FrankfurterResponse? result =
                         await _fxRateService.GetRate(currency.Code);

                    if (result == null)
                        continue;

                    if (!result.Rates.ContainsKey(currency.Code))
                        continue;

                    currency.PreviousRate = currency.CurrentRate;
                    currency.CurrentRate = result.Rates[currency.Code];
                    currency.LastUpdated = DateTime.Now;

                    if (currency.PreviousRate > 0)
                    {
                        currency.ChangePercent =
                            ((currency.CurrentRate - currency.PreviousRate)
                            / currency.PreviousRate) * 100;
                    }
                }
                catch
                {
                    // ข้าม Currency ที่ Error
                }
            }

            TempData["Success"] = "All exchange rates updated.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SeedCurrencies()
        {
            List<CurrencyApiModel> currencies =
                await _fxRateService.GetCurrencies();

            foreach (CurrencyApiModel item in currencies)
            {
                Currency? exist =
                    _service.GetAll()
                            .FirstOrDefault(x => x.Code == item.Code);

                if (exist == null)
                {
                    _service.Add(new Currency
                    {
                        Code = item.Code,
                        Name = item.Name,
                        CurrentRate = 0,
                        PreviousRate = 0,
                        ChangePercent = 0,
                        LastUpdated = DateTime.MinValue,
                        IsActive = true
                    });
                }
            }

            TempData["Success"] =
                "Currency master imported successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}