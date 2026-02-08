using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerRoom.Core.Entities
{
    public class CurrencyRate
    {
        [Key]
        public int Id { get; set; }

        public string CurrencyName { get; set; } = "USD";
        public decimal Amount { get; set; }
        public DateTime RateDate { get; set; } = DateTime.UtcNow;
    }
}
