using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerRoom.Core.Entities
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }
        public int LockerID { get; set; }
        public string BraceletCode { get; set; } = string.Empty;
        public string PaymentType { get; set; } = "Cash";
        public int CurrencyID { get; set; }
        public decimal Amount { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

      
        public DateTime? EndDate { get; set; }
        public string? CreatedBy { get; set; }

        public Locker? Locker { get; set; }
        public CurrencyRate? Currency { get; set; }
        public int? POSCheckNo { get; set; }  // ✅ رقم الفاتورة في الـ POS
    }
}
