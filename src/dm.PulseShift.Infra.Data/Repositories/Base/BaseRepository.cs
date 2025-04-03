using dm.PulseShift.Domain.Entities.Base;
using dm.PulseShift.Domain.Interfaces.Repositories.Base;
using dm.PulseShift.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace dm.PulseShift.Infra.Data.Repositories.Base;

[ExcludeFromCodeCoverage]
public class BaseRepository<TEntity>(PulseShiftDbContext db) : IBaseRepository<TEntity> where TEntity : Entity, new()
{
    protected DbSet<TEntity> _dbSet = db.Set<TEntity>();

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<TEntity?> UpdateAsync(TEntity updated, Guid key)
    {
        if (updated == null)
            return null;

        var existing = await _dbSet.FindAsync(key);
        if (existing != null)
        {
            updated.Id = key;
            db.Entry(existing).CurrentValues.SetValues(updated);
        }

        return existing;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await Task.Yield();
        var entity = new TEntity { Id = id };
        _dbSet.Remove(entity);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id) =>
        await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task<bool> AreThereAsync(Expression<Func<TEntity, bool>> predicate) =>
        await _dbSet.AsNoTracking().AnyAsync(predicate);

    public async Task SaveChangesAsync() =>
        await db.SaveChangesAsync();
}