using SharedKernel;

namespace Accounting.Domain.Events;

public record CardAuthorized(Guid OrderId) : IDomainEvent;