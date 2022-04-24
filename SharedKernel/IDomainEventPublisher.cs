namespace SharedKernel;

public interface IDomainEventPublisher
{
    Task Publish<T>(T domainEvent) where T : IDomainEvent;
}