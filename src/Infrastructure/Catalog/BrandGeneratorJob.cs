using Ardalis.Specification;
using Showmatics.Application.Common.Interfaces;
using Showmatics.Application.Common.Persistence;
using Showmatics.Shared.Notifications;
using Hangfire;
using Hangfire.Console.Extensions;
using Hangfire.Console.Progress;
using Hangfire.Server;
using MediatR;
using Microsoft.Extensions.Logging;
using ShowMatic.Server.Domain.Catalog;
using ShowMatic.Server.Application.Catalog.Customers.Delete;
using ShowMatic.Server.Application.Catalog.Customers.Create;

namespace Showmatics.Infrastructure.Catalog;

public class CustomerGeneratorJob //: ICustomerGeneratorJob
{
    private readonly ILogger<CustomerGeneratorJob> _logger;
    private readonly ISender _mediator;
    private readonly IReadRepository<Customer> _repository;
    private readonly IProgressBarFactory _progressBar;
    private readonly PerformingContext _performingContext;
    private readonly INotificationSender _notifications;
    private readonly ICurrentUser _currentUser;
    private readonly IProgressBar _progress;

    public CustomerGeneratorJob(
        ILogger<CustomerGeneratorJob> logger,
        ISender mediator,
        IReadRepository<Customer> repository,
        IProgressBarFactory progressBar,
        PerformingContext performingContext,
        INotificationSender notifications,
        ICurrentUser currentUser)
    {
        _logger = logger;
        _mediator = mediator;
        _repository = repository;
        _progressBar = progressBar;
        _performingContext = performingContext;
        _notifications = notifications;
        _currentUser = currentUser;
        _progress = _progressBar.Create();
    }

    private async Task NotifyAsync(string message, int progress, CancellationToken cancellationToken)
    {
        _progress.SetValue(progress);
        await _notifications.SendToUserAsync(
            new JobNotification()
            {
                JobId = _performingContext.BackgroundJob.Id,
                Message = message,
                Progress = progress
            },
            _currentUser.GetUserId().ToString(),
            cancellationToken);
    }

    [Queue("notdefault")]
    public async Task GenerateAsync(int nSeed, CancellationToken cancellationToken)
    {
        await NotifyAsync("Your job processing has started", 0, cancellationToken);

        foreach (int index in Enumerable.Range(1, nSeed))
        {
            await _mediator.Send(
                new CreateCustomerRequest
                {
                    Name = $"Customer Random - {Guid.NewGuid()}"
                },
                cancellationToken);

            await NotifyAsync("Progress: ", nSeed > 0 ? (index * 100 / nSeed) : 0, cancellationToken);
        }

        await NotifyAsync("Job successfully completed", 0, cancellationToken);
    }

    [Queue("notdefault")]
    [AutomaticRetry(Attempts = 5)]
    public async Task CleanAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing Job with Id: {jobId}", _performingContext.BackgroundJob.Id);

        var items = await _repository.ListAsync(new RandomCustomersSpec(), cancellationToken);

        _logger.LogInformation("Customers Random: {brandsCount} ", items.Count.ToString());

        foreach (var item in items)
        {
            await _mediator.Send(new DeleteCustomerRequest(item.Id), cancellationToken);
        }

        _logger.LogInformation("All random brands deleted.");
    }
}

public class RandomCustomersSpec : Specification<Customer>
{
    public RandomCustomersSpec() =>
        Query.Where(b => !string.IsNullOrEmpty(b.Name) && b.Name.Contains("Customer Random"));
}