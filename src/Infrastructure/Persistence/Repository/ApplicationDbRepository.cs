using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Showmatics.Application.Common.Persistence;
using Showmatics.Domain.Common.Contracts;
using Showmatics.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
using static Dapper.SqlMapper;

namespace Showmatics.Infrastructure.Persistence.Repository;

// Inherited from Ardalis.Specification's RepositoryBase<T>
public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
        this._dbContext = dbContext;
    }

    public void Detach(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Detached;
    }

    public async Task SetEntityStateModifiedAsync(T entity, CancellationToken cancellation)
    {
        if (entity != null)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellation);
        }
    }

    // We override the default behavior when mapping to a dto.
    // We're using Mapster's ProjectToType here to immediately map the result from the database.
    // This is only done when no Selector is defined, so regular specifications with a selector also still work.
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();
}