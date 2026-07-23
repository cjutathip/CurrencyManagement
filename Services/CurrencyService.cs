using CurrencyManagement.Models;
using Microsoft.Extensions.Configuration;

namespace CurrencyManagement.Services
{
    public class CurrencyService
    {
        private readonly IConfiguration _configuration;
        private readonly List<Currency> _currencies = new();
        private int _nextId = 1;

        public CurrencyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ==========================
        // Get All
        // ==========================
        public List<Currency> GetAll()
        {
            return _currencies;
        }

        // ==========================
        // Get By Id
        // ==========================
        public Currency? GetById(int id)
        {
            return _currencies.FirstOrDefault(c => c.Id == id);
        }

        // ==========================
        // Add
        // ==========================
        public void Add(Currency currency)
        {
            currency.Id = _nextId++;
            _currencies.Add(currency);
        }

        // ==========================
        // Update
        // ==========================
        public void Update(Currency currency)
        {
            Currency? oldCurrency = GetById(currency.Id);

            if (oldCurrency == null)
                return;

            oldCurrency.Code = currency.Code;
            oldCurrency.Name = currency.Name;
            oldCurrency.IsActive = currency.IsActive;
        }
        //===========================
        public void UpdateRate(    Currency currency,    FxRateResult result)
        {
            currency.CurrentRate = result.CurrentRate;

            currency.PreviousRate = result.PreviousRate;

            currency.ChangePercent = result.ChangePercent;

            currency.LastUpdated = result.RateDate;
        }
        // ==========================
        // Delete
        // ==========================
        public void Delete(int id)
        {
            Currency? currency = GetById(id);

            if (currency != null)
            {
                _currencies.Remove(currency);
            }
        }

        // ==========================
        // Check Empty
        // ==========================
        public bool IsEmpty()
        {
            return !_currencies.Any();
        }

        // ==========================
        // Startup Seed
        // ==========================
        public async Task StartupSeedAsync(IFxRateService fxRateService)
        {
            if (!IsEmpty())
                return;
    
            List<CurrencyApiModel> currencies =
                await fxRateService.GetCurrencies();

            FrankfurterResponse? latestRates =
                await fxRateService.GetLatestRates();
            string[] fundCurrencies =
            _configuration
                .GetSection("FundCurrencies")
                .Get<string[]>() ?? Array.Empty<string>();
            foreach (CurrencyApiModel item in currencies)
            {
                if (!fundCurrencies.Contains(item.Code))
                    continue;

                decimal currentRate = 0;

                if (item.Code == "THB")
                {
                    currentRate = 1;
                }
                else if (latestRates != null &&
                         latestRates.Rates.TryGetValue(item.Code, out decimal rate))
                {
                    currentRate = rate;
                }

                Add(new Currency
                {
                    Code = item.Code,
                    Name = item.Name,
                    CurrentRate = currentRate,
                    PreviousRate = currentRate,
                    ChangePercent = 0,
                    LastUpdated = DateTime.Now,
                    IsActive = true
                });
            }
        }
    }
}