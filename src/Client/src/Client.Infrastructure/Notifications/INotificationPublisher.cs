using Showmatics.Shared.Notifications;

namespace Showmatics.Blazor.Client.Infrastructure.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}