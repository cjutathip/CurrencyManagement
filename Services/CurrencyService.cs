using CurrencyManagement.Models;
namespace CurrencyManagement.Services
{
    public class CurrencyService
    {
        private readonly List<Currency> _currencies = new();
        public CurrencyService()
        {
            _currencies.Add(new Currency
            {
                Id = 1,
                Code = "USD",
                Name = "US Dollar",
                IsActive = true
            });

            _currencies.Add(new Currency
            {
                Id = 2,
                Code = "THB",
                Name = "Thai Baht",
                IsActive = true
            });

            _currencies.Add(new Currency
            {
                Id = 3,
                Code = "JPY",
                Name = "Japanese Yen",
                IsActive = true
            });
        }

        public List<Currency> GetAll()
        {
            return _currencies;
        }
        public Currency? GetById(int id)
        {
            return _currencies.FirstOrDefault(x => x.Id == id);
        }
        public void Add(Currency currency)
        {
            currency.Id = _currencies.Max(x => x.Id) + 1;
            _currencies.Add(currency);
        }
        public void Update(Currency currency)
        {
            var item = GetById(currency.Id);

            if (item == null)
                return;

            item.Code = currency.Code;
            item.Name = currency.Name;
            item.IsActive = currency.IsActive;
        }
        public void Delete(int id)
        {
            var item = GetById(id);

            if (item != null)
            {
                _currencies.Remove(item);
            }
        }

    }




}
