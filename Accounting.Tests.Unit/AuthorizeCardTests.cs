using System;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Application.Commands;
using Accounting.Domain.Events;
using Accounting.Infrastructure.Services;
using FluentAssertions;
using SharedKernel;
using Xunit;

namespace Accounting.Tests.Unit;

public class AuthorizeCardTests
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private const string InvalidCardNumber = "INVALID_CARD_NUMBER";
    private const string ValidCardNumber = "VALID_CARD_NUMBER";
    private readonly DomainEventPublisherInMemory _eventPublisher;
    private readonly AuthorizeCardCommandHandler _handler;

    public AuthorizeCardTests()
    {
        _eventPublisher = new DomainEventPublisherInMemory();
        var authorization = new FakeAuthorizationCardService();
        _handler = new AuthorizeCardCommandHandler(_eventPublisher, authorization);
    }

    [Fact]
    public async Task AuthorizeCardShouldPublishCardAuthorizedEvent()
    {
        var command = new AuthorizeCardCommand(OrderId, ValidCardNumber);

        await _handler.HandleAsync(command);

        VerifyPublishedCardAuthorizedEvent();
    }

    [Fact]
    public async Task AuthorizeCardWithInvalidCardNumberShouldPublishAnInvalidCardEvent()
    {
        var command = new AuthorizeCardCommand(OrderId, InvalidCardNumber);
        
        await _handler.HandleAsync(command);
        
        VerifyPublishedCardUnAuthorizedEvent();
    }
    
    private void VerifyPublishedCardUnAuthorizedEvent()
    {
        var publishedEvents = _eventPublisher.OfType<CardNotAuthorized>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().OrderId.Should().Be(OrderId);
    }
    
    private void VerifyPublishedCardAuthorizedEvent()
    {
        var publishedEvents = _eventPublisher.OfType<CardAuthorized>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().OrderId.Should().Be(OrderId);
    }
}