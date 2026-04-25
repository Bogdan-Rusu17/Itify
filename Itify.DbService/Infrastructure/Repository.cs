using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Infrastructure;

public sealed class Repository<TDb>(TDb dbContext) : IRepository<TDb> where TDb : DbContext
{
    public TDb DbContext { get; } = dbContext;

    public async Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity =>
        await DbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<T?> GetAsync<T>(ISpecification<T> spec, CancellationToken cancellationToken = default) where T : BaseEntity =>
        await new SpecificationEvaluator().GetQuery(DbContext.Set<T>().AsQueryable(), spec).FirstOrDefaultAsync(cancellationToken);

    public async Task<TOut?> GetAsync<T, TOut>(ISpecification<T, TOut> spec, CancellationToken cancellationToken = default) where T : BaseEntity =>
        await new SpecificationEvaluator().GetQuery(DbContext.Set<T>().AsQueryable(), spec).FirstOrDefaultAsync(cancellationToken);

    public async Task<int> GetCountAsync<T>(CancellationToken cancellationToken = default) where T : BaseEntity =>
        await DbContext.Set<T>().CountAsync(cancellationToken);

    public async Task<int> GetCountAsync<T>(ISpecification<T> spec, CancellationToken cancellationToken = default) where T : BaseEntity =>
        await new SpecificationEvaluator().GetQuery(DbContext.Set<T>().AsQueryable(), spec).CountAsync(cancellationToken);

    public async Task<List<T>> ListAsync<T>(ISpecification<T> spec, CancellationToken cancellationToken = default) where T : BaseEntity =>
        await new SpecificationEvaluator().GetQuery(DbContext.Set<T>().AsQueryable(), spec).ToListAsync(cancellationToken);

    public async Task<List<TOut>> ListAsync<T, TOut>(ISpecification<T, TOut> spec, CancellationToken cancellationToken = default) where T : BaseEntity =>
        await new SpecificationEvaluator().GetQuery(DbContext.Set<T>().AsQueryable(), spec).ToListAsync(cancellationToken);

    public async Task<PagedResponse<TOut>> PageAsync<T, TOut>(PaginationQueryParams pagination, ISpecification<T, TOut> spec, CancellationToken cancellationToken = default) where T : BaseEntity =>
        new(pagination.Page, pagination.PageSize,
            await new SpecificationEvaluator().GetQuery(DbContext.Set<T>().AsQueryable(), spec).CountAsync(cancellationToken),
            await new SpecificationEvaluator().GetQuery(DbContext.Set<T>().AsQueryable(), spec)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(cancellationToken));

    public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        entity.UpdateTime();
        DbContext.Entry(entity).State = EntityState.Modified;
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<int> DeleteAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var entity = await GetAsync<T>(id, cancellationToken);
        if (entity == null) return 0;
        DbContext.Remove(entity);
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync<T>(ISpecification<T> spec, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var entities = await ListAsync(spec, cancellationToken);
        DbContext.RemoveRange(entities);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entities.Count;
    }
}
