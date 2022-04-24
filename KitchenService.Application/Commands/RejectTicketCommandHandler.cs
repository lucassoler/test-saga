using KitchenService.Domain.Events;
using KitchenService.Domain.Repositories;
using SharedKernel;

namespace KitchenService.Application.Commands;

public class RejectTicketCommandHandler: ICommandHandler<RejectTicketCommand>
{
    private readonly ITicketRepository _repository;
    private readonly IDomainEventPublisher _eventPublisher;

    public RejectTicketCommandHandler(ITicketRepository repository, IDomainEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(RejectTicketCommand command)
    {
        var ticket = await _repository.Get(command.OrderId);
        ticket.Reject();
        await _repository.Save(ticket);
        await _eventPublisher.Publish(new TicketRejected(command.OrderId));
    }
}

public record RejectTicketCommand(Guid OrderId) : ICommand;