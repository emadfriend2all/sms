using ShowMatic.Server.Domain.Catalog;
using Showmatics.Domain.Common.Events;

namespace ShowMatic.Server.Application.Catalog.Customers.Delete;

public class CustomerDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Customer>>
{
    private readonly ILogger<CustomerDeletedEventHandler> _logger;

    public CustomerDeletedEventHandler(ILogger<CustomerDeletedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityDeletedEvent<Customer> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}