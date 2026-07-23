using Microsoft.AspNetCore.Mvc;
using CurrencyManagement.Models;
using CurrencyManagement.Services;

namespace CurrencyManagement.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly CurrencyService _service;
        private readonly IFxRateService _fxRateService;

        public CurrencyController(
    CurrencyService service,
    IFxRateService fxRateService)
        {
            _service = service;
            _fxRateService = fxRateService;
        }

        // ==========================
        // Index
        // ==========================
        //public IActionResult Index()
        //{
        //    return View(_service.GetAll());
        //}
        // ==========================
        // Index for Search
        // ==========================
        public IActionResult Index(string? search, bool? isActive)
        {
            List<Currency> currencies = _service.GetAll();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                currencies = currencies
                    .Where(c =>
                        c.Code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Status Filter
            if (isActive.HasValue)
            {
                currencies = currencies
                    .Where(c => c.IsActive == isActive.Value)
                    .ToList();
            }

            ViewBag.Search = search;
            ViewBag.IsActive = isActive;

            return View(currencies);
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
            // แปลง Code เป็นตัวพิมพ์ใหญ่
            currency.Code = currency.Code.Trim().ToUpper();

            // ตรวจสอบ Code ซ้ำ
            if (_service.GetAll().Any(c => c.Code == currency.Code))
            {
                ModelState.AddModelError(
                    "Code",
                    "Currency Code already exists.");
            }

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
            // แปลง Code เป็นตัวพิมพ์ใหญ่
            currency.Code = currency.Code.Trim().ToUpper();

            // ตรวจสอบว่ามี Currency อื่นใช้ Code นี้หรือไม่
            bool duplicate =
                _service.GetAll().Any(c =>
                    c.Id != currency.Id &&
                    c.Code == currency.Code);

            if (duplicate)
            {
                ModelState.AddModelError(
                    "Code",
                    "Currency Code already exists.");
            }

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

            try
            {
                FxRateResult? result =
                    await _fxRateService.GetLatestAndPreviousRate(currency.Code);

                if (result == null)
                {
                    TempData["Error"] = "Cannot retrieve exchange rate.";
                    return RedirectToAction(nameof(Index));
                }

                _service.UpdateRate(currency, result);

                TempData["Success"] = "Exchange rate updated successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Cannot connect to Exchange Rate API.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // Refresh All
        // ==========================
        public async Task<IActionResult> RefreshAll()
        {
            foreach (Currency currency in _service.GetAll())
            {
                try
                {
                    FxRateResult? result =
                        await _fxRateService.GetLatestAndPreviousRate(currency.Code);

                    if (result == null)
                        continue;

                    //currency.CurrentRate = result.CurrentRate;
                    //currency.PreviousRate = result.PreviousRate;
                    //currency.ChangePercent = result.ChangePercent;
                    //currency.LastUpdated = result.RateDate;
                    _service.UpdateRate(currency, result);
                }
                catch
                {
                    // ข้ามสกุลเงินที่ Error
                }
            }

            TempData["Success"] = "All exchange rates updated successfully.";

            return RedirectToAction(nameof(Index));
        }


    }
}