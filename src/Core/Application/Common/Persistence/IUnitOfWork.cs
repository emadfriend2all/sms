using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showmatics.Application.Common.Persistence;
public interface IUnitOfWork : IDisposable, ITransientService
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}
