using Contracts;

namespace Orchestration.ConsumerService.Domain.Events;

public record ConsumerVerified(Guid ConsumerId) : IDomainEvent;