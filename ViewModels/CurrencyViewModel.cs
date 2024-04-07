using CurrencyMonitoringWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CurrencyMonitoringWebApp.ViewModels
{
    public class CurrencyViewModel
    {
        public SelectList FromCurrencies { get; set; } = new SelectList(new List<Currency>(), "Id", "Description");
        public SelectList ToCurrencies { get; set; } = new SelectList(new List<Currency>(), "Id", "Description");
    }
}
