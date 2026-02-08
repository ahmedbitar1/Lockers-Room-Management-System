using LockerRoom.Core.Entities;
using LockerRoom.Core.Interfaces;
using LockerRoom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LockerRoom.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MainDbContext _context;

        public IRepository<Locker> Lockers { get; }
        public IRepository<LockerGroup> LockerGroups { get; }
        public IRepository<Booking> Bookings { get; }
        public IRepository<CurrencyRate> CurrencyRates { get; }
        public IRepository<LockerLog> LockerLogs { get; } 

        public UnitOfWork(MainDbContext context)
        {
            _context = context;

            Lockers = new Repository<Locker>(_context);
            LockerGroups = new Repository<LockerGroup>(_context);
            Bookings = new Repository<Booking>(_context);
            CurrencyRates = new Repository<CurrencyRate>(_context);
            LockerLogs = new Repository<LockerLog>(_context); 
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task ExecuteSqlAsync(string sql)
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
