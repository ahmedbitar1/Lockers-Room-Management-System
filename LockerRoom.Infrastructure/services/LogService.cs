using LockerRoom.Core.Entities;
using LockerRoom.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace LockerRoom.Infrastructure.Services
{
    public class LogService
    {
        private readonly MainDbContext _context;

        public LogService(MainDbContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(string actionType, string username, int? lockerId = null,
            int? bookingId = null, string? braceletCode = null, decimal? amount = null,
            string? paymentType = null, string? details = null, bool? prevIsActive = null,
            DateTime? prevEndDate = null)
        {
            var log = new LockerLog
            {
                ActionType = actionType,
                Username = username,
                LockerID = lockerId,
                BookingID = bookingId,
                BraceletCode = braceletCode,
                Amount = amount,
                PaymentType = paymentType,
                Details = details,
                PreviousIsActive = prevIsActive,
                PreviousEndDate = prevEndDate,
                CreatedBy = username,
                ActionDate = DateTime.Now
            };

            _context.LockerLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
