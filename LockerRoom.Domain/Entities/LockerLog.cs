using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockerRoom.Core.Entities
{
    public class LockerLog
    {
        [Key]
        public int LogID { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string ActionType { get; set; } = string.Empty;

        public string? Username { get; set; }

        public int? LockerID { get; set; }

        public int? BookingID { get; set; }

        public string? BraceletCode { get; set; }

        public int? CurrencyID { get; set; }

        public decimal? Amount { get; set; }

        public string? PaymentType { get; set; }

        public string? Details { get; set; }

        public bool? PreviousIsActive { get; set; }

        public DateTime? PreviousEndDate { get; set; }

        public string? CreatedBy { get; set; }
    }
}
