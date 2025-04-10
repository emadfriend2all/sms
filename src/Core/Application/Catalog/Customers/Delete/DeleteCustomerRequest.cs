using ShowMatic.Server.Domain.Catalog;
using Showmatics.Domain.Common.Events;

namespace ShowMatic.Server.Application.Catalog.Customers.Delete;

public class DeleteCustomerRequest : IRequest<int>
{
    public int Id { get; set; }

    public DeleteCustomerRequest(int id) => Id = id;
}

public class DeleteCustomerRequestHandler : IRequestHandler<DeleteCustomerRequest, int>
{
    private readonly IRepository<Customer> _repository;
    private readonly IStringLocalizer<DeleteCustomerRequestHandler> _localizer;

    public DeleteCustomerRequestHandler(IRepository<Customer> repository, IStringLocalizer<DeleteCustomerRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<int> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = product ?? throw new NotFoundException(_localizer["product.notfound"]);

        // Add Domain Events to be raised after the commit
        product.DomainEvents.Add(EntityDeletedEvent.WithEntity(product));

        await _repository.DeleteAsync(product, cancellationToken);

        return request.Id;
    }
}