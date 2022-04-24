using SharedKernel;

namespace KitchenService.Domain.Events;

public record TicketCreated(Guid OrderId) : IDomainEvent;