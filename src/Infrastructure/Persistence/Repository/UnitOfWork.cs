using Microsoft.EntityFrameworkCore;
using Showmatics.Application.Common.Persistence;
using Showmatics.Infrastructure.Persistence.Context;

namespace Showmatics.Infrastructure.Persistence.Repository;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void BeginTransaction()
    {
        _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        _dbContext.SaveChanges();
        _dbContext.Database.CommitTransaction();
    }

    public void Rollback()
    {
        _dbContext.Database.RollbackTransaction();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}