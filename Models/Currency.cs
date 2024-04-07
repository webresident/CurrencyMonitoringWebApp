using System.ComponentModel.DataAnnotations;

namespace CurrencyMonitoringWebApp.Models
{
    public class Currency
    {
        [Key]
        public string Id { get; set; }
        public string? Description { get; set; }
    }
}
