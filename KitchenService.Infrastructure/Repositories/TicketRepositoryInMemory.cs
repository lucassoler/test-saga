using KitchenService.Domain.Aggregates;
using KitchenService.Domain.Exceptions;
using KitchenService.Domain.Repositories;

namespace KitchenService.Infrastructure.Repositories;

public class TicketRepositoryInMemory : ITicketRepository
{
    private readonly IList<Ticket> _ticketPersisted;

    public TicketRepositoryInMemory(List<Ticket>? tickets = null)
    {
        _ticketPersisted = tickets ?? new List<Ticket>();
    }

    public Task Save(Ticket ticket)
    {
        var index = _ticketPersisted.IndexOf(ticket);
        if (index == -1)
        {
            _ticketPersisted.Remove(ticket);
        }
        
        _ticketPersisted.Add(ticket);
        return Task.CompletedTask;
    }

    public Task<Ticket> Get(Guid orderId)
    
    {
        var result = _ticketPersisted.FirstOrDefault(x => x.OrderId == orderId);

        if (result is null)
        {
            throw new TicketNotFoundException();
        }
        
        return Task.FromResult(result);
    }
}