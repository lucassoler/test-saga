namespace Contracts;

public interface IDomainEventPublisher
{
    Task Publish<T>(T domainEvent) where T : IDomainEvent;
}