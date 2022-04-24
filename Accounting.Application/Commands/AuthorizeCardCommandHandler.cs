using Accounting.Domain.Events;
using Accounting.Domain.Services;
using SharedKernel;

namespace Accounting.Application.Commands;

public class AuthorizeCardCommandHandler: ICommandHandler<AuthorizeCardCommand>
{
    private readonly IDomainEventPublisher _eventPublisher;
    private readonly IAuthorizationCardService _authorization;

    public AuthorizeCardCommandHandler(IDomainEventPublisher eventPublisher,
        IAuthorizationCardService authorization)
    {
        _eventPublisher = eventPublisher;
        _authorization = authorization;
    }

    public async Task HandleAsync(AuthorizeCardCommand command)
    {
        var result = await _authorization.Authorize(command.CardNumber);

        if (!result.IsAuthorized)
        {
            await _eventPublisher.Publish(new CardNotAuthorized(command.OrderId));
        }
        else
        {
            await _eventPublisher.Publish(new CardAuthorized(command.OrderId));
        }
    }
}

public record AuthorizeCardCommand(Guid OrderId, string CardNumber) : ICommand;