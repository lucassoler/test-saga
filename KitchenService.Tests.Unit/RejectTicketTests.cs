using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KitchenService.Application.Commands;
using KitchenService.Domain.Aggregates;
using KitchenService.Domain.Events;
using KitchenService.Infrastructure.Repositories;
using SharedKernel;
using Xunit;

namespace KitchenService.Tests.Unit;

public class RejectTicketTests
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private readonly RejectTicketCommandHandler _handler;
    private readonly RejectTicketCommand _command;
    private readonly DomainEventPublisherInMemory _eventPublisher;
    private readonly TicketRepositoryInMemory _repository;

    public RejectTicketTests()
    {
        _eventPublisher = new DomainEventPublisherInMemory();
        _repository = new TicketRepositoryInMemory(new List<Ticket> { new(OrderId)});
        _handler = new RejectTicketCommandHandler(_repository, _eventPublisher);
        _command = CreateCommand(OrderId);
    }
    
    [Fact]
    public async Task RejectATicketShouldPublishATicketRejectedEvent()
    {
        await _handler.HandleAsync(_command);
        VerifyPublishedEvent();
    }

    [Fact]
    public async Task RejectATicketShouldChangeIsStateToRejected()
    {
        await _handler.HandleAsync(_command);
        await VerifyTicketPersisted(OrderId);
    }    
    
    private async Task VerifyTicketPersisted(Guid orderId)
    {
        var ticketPersisted = await _repository.Get(orderId);
        ticketPersisted.Should().NotBeNull();
        ticketPersisted.OrderId.Should().Be(OrderId);
        ticketPersisted.Status.Should().Be(TicketStatuses.Rejected);
    }
    
    private void VerifyPublishedEvent()
    {
        var publishedEvents = _eventPublisher.OfType<TicketRejected>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().OrderId.Should().Be(OrderId);
    }

    private static RejectTicketCommand CreateCommand(Guid orderId)
    {
        return new RejectTicketCommand(orderId);
    }
    
}