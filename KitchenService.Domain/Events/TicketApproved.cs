using SharedKernel;

namespace KitchenService.Domain.Events;

public record TicketApproved(Guid OrderId) : IDomainEvent;