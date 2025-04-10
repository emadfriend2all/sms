using ShowMatic.Server.Application.Catalog.Customers.Specifications;
using ShowMatic.Server.Domain.Catalog;
using Showmatics.Application.Common.Exporters;

namespace ShowMatic.Server.Application.Catalog.Customers.Export;

public class ExportCustomersRequest : BaseFilter, IRequest<Stream>
{
    public bool? IsCompany { get; set; }
}

public class ExportCustomersRequestHandler : IRequestHandler<ExportCustomersRequest, Stream>
{
    private readonly IReadRepository<Customer> _repository;
    private readonly IExcelWriter _excelWriter;

    public ExportCustomersRequestHandler(IReadRepository<Customer> repository, IExcelWriter excelWriter)
    {
        _repository = repository;
        _excelWriter = excelWriter;
    }


    public async Task<Stream> Handle(ExportCustomersRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportCustomersWithContactsSpecification(request);

        var list = await _repository.ListAsync(spec, cancellationToken);
        var data = list.Select(x => new CustomerExportResponse
        {
            Name = x.Name,
            IdentityNumber = x.IdentityNumber,
            TaxNumber = x.TaxNumber,
            RegistrationNo = x.RegistrationNo,
            Address = x.Address,
            IsCompany = x.IsCompany,
        });
        return _excelWriter.WriteToStream(data.ToList());
    }
}
