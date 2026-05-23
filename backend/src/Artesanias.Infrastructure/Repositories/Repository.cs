using Artesanias.Domain.Interfaces;
using Artesanias.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Artesanias.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ArtesaniasDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ArtesaniasDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _dbSet.AddAsync(entity, ct);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct) is not null;
}
