using System.Text.Json;
using CurrencyManagement.Models;

namespace CurrencyManagement.Services
{
    public class FxRateService : IFxRateService
    {
        private readonly HttpClient _httpClient;

        public FxRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FrankfurterResponse?> GetRate(string currency)
        {
            string requestUrl =
                $"https://api.frankfurter.dev/v1/latest?base=THB&symbols={currency.Trim().ToUpper()}";

            HttpResponseMessage response =
                await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            FrankfurterResponse? result =
                JsonSerializer.Deserialize<FrankfurterResponse>(json, options);

            return result;
        }
        public async Task<FrankfurterResponse?> GetRate(
            string fromCurrency,
            string toCurrency)
        {
            string requestUrl =
                $"https://api.frankfurter.dev/v1/latest?base={fromCurrency.Trim().ToUpper()}&symbols={toCurrency.Trim().ToUpper()}";

            HttpResponseMessage response =
                await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            FrankfurterResponse? result =
                JsonSerializer.Deserialize<FrankfurterResponse>(
                    json,
                    options);

            return result;
        }
        public async Task<List<CurrencyApiModel>> GetCurrencies()
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    "https://api.frankfurter.dev/v1/currencies");

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync();

            Dictionary<string, string>? data =
                JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            List<CurrencyApiModel> result =
                new List<CurrencyApiModel>();

            if (data != null)
            {
                foreach (KeyValuePair<string, string> item in data)
                {
                    result.Add(new CurrencyApiModel
                    {
                        Code = item.Key,
                        Name = item.Value
                    });
                }
            }

            return result;
        }
        public async Task<FrankfurterResponse?> GetLatestRates()
        {
            string requestUrl =
                "https://api.frankfurter.dev/v1/latest?base=THB";

            HttpResponseMessage response =
                await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            FrankfurterResponse? result =
                JsonSerializer.Deserialize<FrankfurterResponse>(
                    json,
                    options);

            return result;
        }
        public async Task<FrankfurterTimeSeriesResponse?> GetPreviousRate(
    string currency)
        {
            DateTime endDate = DateTime.Today;

            DateTime startDate = endDate.AddDays(-7);

            string requestUrl =
                $"https://api.frankfurter.dev/v1/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base=THB&symbols={currency}";

            HttpResponseMessage response =
                await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync();

            JsonSerializerOptions options =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            FrankfurterTimeSeriesResponse? result =
                JsonSerializer.Deserialize<FrankfurterTimeSeriesResponse>(
                    json,
                    options);

            return result;
        }
        public async Task<FxRateResult?> GetLatestAndPreviousRate(
    string currency)
        {
            if (currency.Trim().ToUpper() == "THB")
            {
                return new FxRateResult
                {
                    CurrentRate = 1,
                    PreviousRate = 1,
                    ChangePercent = 0,
                    RateDate = DateTime.Today
                };
            }

            FrankfurterResponse? latest = await GetRate(currency);
            Console.WriteLine($"Latest Date : {latest?.Date}");
            Console.WriteLine($"Latest Rate : {latest?.Rates[currency]}");

            if (latest == null)
                return null;

            if (!latest.Rates.ContainsKey(currency))
                return null;

            decimal latestRate =
                latest.Rates[currency];

            FrankfurterTimeSeriesResponse? history =
                await GetPreviousRate(currency);

            decimal previousRate =
                latestRate;

            if (history != null)
            {
                List<string> dates =
                    history.Rates.Keys
                        .OrderByDescending(x => x)
                        .ToList();

                foreach (string date in dates)
                {
                    if (date == latest.Date)
                        continue;

                    if (history.Rates[date]
                        .ContainsKey(currency))
                    {
                        previousRate =
                            history.Rates[date][currency];
                        Console.WriteLine($"Previous Date : {date}");
                        Console.WriteLine($"Previous Rate : {previousRate}");
                        break;
                    }
                }
            }

            decimal change = 0;

            if (previousRate > 0)
            {
                change =
                    ((latestRate - previousRate)
                    / previousRate) * 100;
            }

            return new FxRateResult
            {
                CurrentRate = latestRate,
                PreviousRate = previousRate,
                ChangePercent = change,
                RateDate = DateTime.Parse(latest.Date)
            };
        }
    }
}