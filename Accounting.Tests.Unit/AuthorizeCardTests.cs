using System;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Application.Commands;
using Accounting.Domain.Events;
using FluentAssertions;
using SharedKernel;
using Xunit;

namespace Accounting.Tests.Unit;

public class AuthorizeCardTests
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private readonly DomainEventPublisherInMemory _eventPublisher;

    public AuthorizeCardTests()
    {
        _eventPublisher = new DomainEventPublisherInMemory();
    }

    [Fact]
    public async Task AuthorizeCardShouldPublishCardAuthorizedEvent()
    {
        var command = new AuthorizeCardCommand(OrderId);
        var handler = new AuthorizeCardCommandHandler(_eventPublisher);

        await handler.HandleAsync(command);

        VerifyPublishedEvent();
    }
    
    private void VerifyPublishedEvent()
    {
        var publishedEvents = _eventPublisher.OfType<CardAuthorized>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().OrderId.Should().Be(OrderId);
    }
}