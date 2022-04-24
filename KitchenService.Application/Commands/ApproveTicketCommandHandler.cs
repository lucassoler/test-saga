using Contracts;
using KitchenService.Domain.Events;
using KitchenService.Domain.Repositories;

namespace KitchenService.Application.Commands;

public class ApproveTicketCommandHandler : ICommandHandler<ApproveTicketCommand>
{
    private readonly ITicketRepository _repository;
    private readonly IDomainEventPublisher _eventPublisher;

    public ApproveTicketCommandHandler(ITicketRepository repository,
        IDomainEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(ApproveTicketCommand command)
    {
        var ticket = await _repository.Get(command.OrderId);
        ticket.Approve();
        await _repository.Save(ticket);
        await _eventPublisher.Publish(new TicketApproved(command.OrderId));
    }
}

public record ApproveTicketCommand(Guid OrderId) : ICommand;