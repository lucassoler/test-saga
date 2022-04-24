using SharedKernel;

namespace KitchenService.Domain.Events;

public record TicketRejected(Guid OrderId) : IDomainEvent;