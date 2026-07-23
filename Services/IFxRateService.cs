using CurrencyManagement.Models;

namespace CurrencyManagement.Services
{
    public interface IFxRateService
    {
        Task<FrankfurterResponse?> GetRate(string currency);

        Task<FrankfurterResponse?> GetRate(
            string fromCurrency,
            string toCurrency);

        Task<List<CurrencyApiModel>> GetCurrencies();

        Task<FrankfurterResponse?> GetLatestRates();

        Task<FrankfurterTimeSeriesResponse?> GetPreviousRate(
            string currency);

        Task<FxRateResult?> GetLatestAndPreviousRate(
            string currency);
    }
}