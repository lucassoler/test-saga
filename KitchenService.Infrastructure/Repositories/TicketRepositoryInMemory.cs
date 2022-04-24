using KitchenService.Domain.Aggregates;
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
        return Task.Delay(0);
    }

    public Task<Ticket> Get(Guid orderId)
    {
        return Task.FromResult(_ticketPersisted.First(x => x.OrderId == orderId));
    }
}