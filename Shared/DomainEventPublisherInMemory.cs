namespace Contracts;

public class DomainEventPublisherInMemory: IDomainEventPublisher
{
    private readonly List<IDomainEvent> _publishedEvents = new();

    public Task Publish<T>(T domainEvent) where T : IDomainEvent
    {
        _publishedEvents.Add(domainEvent);
        return Task.Delay(0);
    }

    public bool ShouldHavePublished<T>(IDomainEvent domainEvent) where T : IDomainEvent
    {
        return _publishedEvents.OfType<T>().Any();
    }

    public IEnumerable<T> OfType<T>()
    {
        return _publishedEvents.OfType<T>();
    }
}