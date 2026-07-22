using System.Text.Json;
using CurrencyManagement.Models;

namespace CurrencyManagement.Services
{
    public class FxRateService
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
    }
}