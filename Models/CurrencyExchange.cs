using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CurrencyMonitoringWebApp.Models
{
    public class CurrencyExchange
    {
        [Key]
        public long Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExchangeDate { get; set; }
        [ForeignKey("Currency")]
        public string? FromCurrency { get; set; }
        [ForeignKey("Currency")]
        public string? ToCurrency { get; set; }
        public double? ExchangeRate { get; set; }
    }
}
