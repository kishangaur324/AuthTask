using AuthTask.Application.Interfaces;
using AuthTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthTask.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _authDbContext;
        private IDbContextTransaction? _transaction = null;

        public UnitOfWork(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _transaction = await _authDbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction");

            await _transaction!.CommitAsync(cancellationToken);
            await _transaction!.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
                return;

            await _transaction!.RollbackAsync(cancellationToken);
            await _transaction!.DisposeAsync();
            _transaction = null;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _authDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
