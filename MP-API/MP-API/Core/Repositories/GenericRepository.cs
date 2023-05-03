using Microsoft.EntityFrameworkCore;
using MP_API.Core.Interfaces;
using MP_API.Data;
using System.Linq.Expressions;

namespace MP_API.Core.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DataContext _context;
    internal DbSet<T> _dbSet;

    public GenericRepository(DataContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        // AsNoTracking() to prevent tracking multiple entities from DB
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().AnyAsync(predicate);
    }

    public virtual async Task<bool> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        _context.Update(entity);
        return true; // Remember to call Save() after this
    }
    public virtual async Task<bool> DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return true;
    }

    //RemoveRange
}
