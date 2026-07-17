using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IRepository{TEntity}"/>. Entity-specific
/// repositories inherit this for CRUD and add their own query methods on top.
/// </summary>
public class RepositoryBase<TEntity>(ApplicationDbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext Context = context;

    protected DbSet<TEntity> DbSet => Context.Set<TEntity>();

    public virtual Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(TEntity entity) => DbSet.Update(entity);

    public virtual void Remove(TEntity entity) => DbSet.Remove(entity);
}
