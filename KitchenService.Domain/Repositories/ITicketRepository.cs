using KitchenService.Domain.Aggregates;

namespace KitchenService.Domain.Repositories;

public interface ITicketRepository
{
    Task Save(Ticket ticket);
    Task<Ticket> Get(Guid orderId);
}