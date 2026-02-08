using LockerRoom.Core.Entities;

namespace LockerRoom.Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<Locker> Lockers { get; }
        IRepository<LockerGroup> LockerGroups { get; }
        IRepository<Booking> Bookings { get; }
        IRepository<CurrencyRate> CurrencyRates { get; }

    
        IRepository<LockerLog> LockerLogs { get; }

        Task<int> CompleteAsync();
        Task ExecuteSqlAsync(string sql);
    }
}

