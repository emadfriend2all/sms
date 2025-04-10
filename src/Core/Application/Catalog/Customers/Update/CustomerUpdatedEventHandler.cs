using ShowMatic.Server.Domain.Catalog;
using Showmatics.Domain.Common.Events;

namespace ShowMatic.Server.Application.Catalog.Customers.Update;

public class CustomerUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Customer>>
{
    private readonly ILogger<CustomerUpdatedEventHandler> _logger;

    public CustomerUpdatedEventHandler(ILogger<CustomerUpdatedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityUpdatedEvent<Customer> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}