using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using FluentAssertions;
using Orchestration.Commands;
using Orchestration.ConsumerService.Domain.Events;
using Xunit;

namespace Tests.Consumer.Unit.ConsumerService;

public class VerifyConsumerTests
{
    private static readonly Guid ConsumerId = Guid.NewGuid();

    [Fact]
    public async Task VerifyAValidConsumerShouldPublishAConsumerVerifiedEvent()
    {
        var eventPublisher = new DomainEventPublisherInMemory();
        var command = new VerifyConsumerCommand(ConsumerId);
        var handler = new VerifyConsumerCommandHandler(eventPublisher);
        
        await handler.HandleAsync(command);
        
        var publishedEvents = eventPublisher.OfType<ConsumerVerified>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().ConsumerId.Should().Be(ConsumerId);
    }
}