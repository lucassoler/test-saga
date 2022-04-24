using SharedKernel;

namespace Accounting.Domain.Events;

public record CardNotAuthorized(Guid OrderId) : IDomainEvent;