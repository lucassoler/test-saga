using SharedKernel;

namespace OrderService.Domain.Events;

public record OrderCreated(Guid OrderId) : IDomainEvent;