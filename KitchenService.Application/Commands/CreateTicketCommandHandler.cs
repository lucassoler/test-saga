using KitchenService.Domain.Aggregates;
using KitchenService.Domain.Events;
using KitchenService.Domain.Repositories;
using SharedKernel;

namespace KitchenService.Application.Commands;

public class CreateTicketCommandHandler: ICommandHandler<CreateTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IDomainEventPublisher _eventPublisher;

    public CreateTicketCommandHandler(ITicketRepository ticketRepository, IDomainEventPublisher eventPublisher)
    {
        _ticketRepository = ticketRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(CreateTicketCommand command)
    {
        var ticket = new Ticket(command.OrderId);
        await _ticketRepository.Save(ticket);
        await _eventPublisher.Publish(new TicketCreated(command.OrderId));
    }
}

public record CreateTicketCommand(Guid OrderId) : ICommand;