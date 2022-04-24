using Accounting.Domain.Events;
using SharedKernel;

namespace Accounting.Application.Commands;

public class AuthorizeCardCommandHandler: ICommandHandler<AuthorizeCardCommand>
{
    private readonly IDomainEventPublisher _eventPublisher;

    public AuthorizeCardCommandHandler(IDomainEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(AuthorizeCardCommand command)
    {
        await _eventPublisher.Publish(new CardAuthorized(command.OrderId));
    }
}

public record AuthorizeCardCommand(Guid OrderId) : ICommand;