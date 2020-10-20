using Device.Persistance.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Polly;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Device.Persistance
{
    public class DeviceDbContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "device";
        public DeviceDbContext(DbContextOptions<DeviceDbContext> options)
           : base(options)
        {

        }

        public DbSet<Devices.Domain.AggregatesModel.Device> Devices { get; set; }


        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction => _currentTransaction;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Devices.Domain.AggregatesModel.Device>().HasKey(m => m.DeviceId);
            modelBuilder.Entity<Devices.Domain.AggregatesModel.Device>().ToTable("Devices");
            base.OnModelCreating(modelBuilder);
        }

        public void MigrateDB()
        {
            Policy
                .Handle<Exception>()
                .WaitAndRetry(10, r => TimeSpan.FromSeconds(10))
                .Execute(() => Database.Migrate());
        }

        public async Task BeginTransactionAsync()
        {
            _currentTransaction = _currentTransaction ?? await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
