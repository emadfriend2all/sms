
using Showmatics.Shared.Notifications;

namespace Showmatics.Blazor.Client.Infrastructure.Notifications;

public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;