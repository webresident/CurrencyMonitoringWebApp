using CurrencyMonitoringWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CurrencyMonitoringWebApp.ViewModels
{
    public class CurrencyExchangeViewModel
    {
        public IEnumerable<CurrencyExchange> CurrencyExchanges { get; set; } = new List<CurrencyExchange>();
        public SelectList FromCurrencies { get; set; } = new SelectList(new List<Currency>(), "Id", "Description");
        public SelectList ToCurrencies { get; set; } = new SelectList(new List<Currency>(), "Id", "Description");
    }
}
