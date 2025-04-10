namespace Showmatics.Application.Common.Interfaces;

public interface ICompanyInitializer : ITransientService
{
    Task InitializeAsync(int companyId, CancellationToken cancellationToken);
}