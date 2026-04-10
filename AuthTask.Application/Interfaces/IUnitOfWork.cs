namespace AuthTask.Application.Interfaces
{
    /// <summary>
    /// Unit-of-work contract for transaction and save lifecycle management.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task BeginTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task CommitAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Rolls back the current transaction when present.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RollbackAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Persists pending changes to the data store.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of state entries written to the store.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
