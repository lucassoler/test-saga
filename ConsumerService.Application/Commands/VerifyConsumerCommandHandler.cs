using Contracts;
using Orchestration.ConsumerService.Domain.Events;

namespace Orchestration.Commands;

public class VerifyConsumerCommandHandler : ICommandHandler<VerifyConsumerCommand>
{
    private readonly IDomainEventPublisher _eventPublisher;

    public VerifyConsumerCommandHandler(IDomainEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(VerifyConsumerCommand command)
    {
        await _eventPublisher.Publish(new ConsumerVerified(command.ConsumerId));
    }
}

public record VerifyConsumerCommand(Guid ConsumerId): ICommand;