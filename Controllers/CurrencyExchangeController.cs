using CurrencyMonitoringWebApp.Data;
using CurrencyMonitoringWebApp.Models;
using CurrencyMonitoringWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace CurrencyMonitoringWebApp.Controllers
{
    public class CurrencyExchangeController : Controller
    {
        private ApplicationDbContext _context;
        private int _pageSize = 35;
        public CurrencyExchangeController(ApplicationDbContext context)
        {
            _context = context;
            if (!_context.Currencies.Any())
            {
                List<Currency> currencies = new List<Currency>() {
                    new Currency {Id = "EUR", Description = "Euro"},
                    new Currency {Id = "USD", Description = "United States Dollar"},
                    new Currency {Id = "JPY", Description = "Japanese Yen"},
                    new Currency {Id = "GBP", Description = "Pound Sterling"},
                    new Currency {Id = "AUD", Description = "Australian Dollar"},
                    new Currency {Id = "CAD", Description = "Canadian Dollar"},
                };

                _context.Database.OpenConnection();
                try
                {
                    _context.Currencies.AddRange(currencies);
                    _context.SaveChanges();
                }
                finally
                {
                    _context.Database.CloseConnection();
                }
            }

            List<Currency> toCurrencies = _context.Currencies.ToList();
            toCurrencies.RemoveAll(c => c.Id == "EUR");

            foreach (Currency cur in toCurrencies)
            {
                UpdateDailyExchangeRate("EUR", cur.Id, DateOnly.FromDateTime(DateTime.Today.AddDays(-90)), DateOnly.FromDateTime(DateTime.Today));
            }

        }

        public IActionResult Index(string? fromCurrency, string? toCurrency)
        {
            IQueryable<CurrencyExchange> currencyExchanges = _context.CurrencyExchanges;

            List<Currency> fromCurrencies = _context.Currencies.Where(c => c.Id == "EUR").ToList();
            List<Currency> toCurrencies = _context.Currencies.ToList();
            toCurrencies.RemoveAll(c => c.Id == "EUR");

            CurrencyExchangeViewModel viewModel = new CurrencyExchangeViewModel()
            {
                CurrencyExchanges = currencyExchanges.ToList(),
                FromCurrencies = new SelectList(fromCurrencies, "Id", "Description", fromCurrency),
                ToCurrencies = new SelectList(toCurrencies, "Id", "Description", toCurrency)
            };

            return View(viewModel);
        }


        public IActionResult GetPage(string? fromCurrency, string? toCurrency, int page)
        {
            var itemsToSkip = page * _pageSize;

            var exchanges = _context.CurrencyExchanges.Where(c => c.FromCurrency == fromCurrency && c.ToCurrency == toCurrency);
            var data = exchanges.OrderByDescending(t => t.ExchangeDate).Skip(itemsToSkip).
                    Take(_pageSize).ToList();
            return PartialView("IndexPartialView", data);
        }

        private void UpdateDailyExchangeRate(string fromCurrency, string toCurrency, DateOnly startPeriod, DateOnly endPeriod)
        {
            int count = -1;
            string url = "https://data-api.ecb.europa.eu/service/data/EXR/D";


            url = $"{url}.{toCurrency}.{fromCurrency}.SP00.A?startPeriod={startPeriod.ToString("yyyy-MM-dd")}&endPeriod={endPeriod.ToString("yyyy-MM-dd")}";
            XmlTextReader reader = new XmlTextReader(url);

            List<CurrencyExchange> exchanges = new List<CurrencyExchange>();
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "generic:Obs":
                            exchanges.Add(new CurrencyExchange { FromCurrency = fromCurrency, ToCurrency = toCurrency });
                            count++;
                            break;
                        case "generic:ObsDimension":
                            reader.MoveToAttribute("value");
                            exchanges[count].ExchangeDate = DateTime.Parse(reader.Value);
                            break;
                        case "generic:ObsValue":
                            reader.MoveToAttribute("value");
                            exchanges[count].ExchangeRate = double.Parse(reader.Value);
                            break;
                    }
                }
            }

            _context.Database.OpenConnection();
            try
            {
                for (int i = 0; i < exchanges.Count; i++)
                {
                    bool exists = false;

                    exists = _context.CurrencyExchanges.Any(e => e.ExchangeDate == exchanges[i].ExchangeDate && e.FromCurrency == exchanges[i].FromCurrency && e.ToCurrency == exchanges[i].ToCurrency);
                    if (!exists)
                    {
                        _context.CurrencyExchanges.Add(exchanges[i]);
                        _context.SaveChanges();
                    }
                }
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }
    }
}
