using Contracts;
using KitchenService.Domain.Repositories;

namespace KitchenService.Application.Commands;

public class ApproveTicketCommandHandler : ICommandHandler<ApproveTicketCommand>
{
    private readonly ITicketRepository _repository;

    public ApproveTicketCommandHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ApproveTicketCommand command)
    {
        var ticket = await _repository.Get(command.OrderId);
        ticket.Approve();
        await _repository.Save(ticket);
    }
}

public record ApproveTicketCommand(Guid OrderId) : ICommand;