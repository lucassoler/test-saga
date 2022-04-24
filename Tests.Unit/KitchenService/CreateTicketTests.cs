using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using FluentAssertions;
using KitchenService.Application.Commands;
using KitchenService.Domain.Aggregates;
using KitchenService.Domain.Events;
using KitchenService.Infrastructure.Repositories;
using Xunit;

namespace Tests.Consumer.Unit.KitchenService;

public class CreateTicketTests
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private readonly DomainEventPublisherInMemory _eventPublisher;
    private readonly CreateTicketCommand _command;
    private readonly CreateTicketCommandHandler _handler;
    private readonly TicketRepositoryInMemory _repository;

    public CreateTicketTests()
    {
        _eventPublisher = new DomainEventPublisherInMemory();
        _repository = new TicketRepositoryInMemory();
        _command = new CreateTicketCommand(OrderId);
        _handler = new CreateTicketCommandHandler(_repository, _eventPublisher);
    }

    [Fact]
    public async Task CreateTicketInKitchenShouldCreateATicketWaitingApproval()
    {
        await _handler.HandleAsync(_command);
        await VerifyTicketPersisted(OrderId);
    }

    [Fact]
    public async Task CreateTicketInKitchenShouldPublishTicketCreatedEvent()
    {
        await _handler.HandleAsync(_command);
        VerifyPublishedEvent();
    }

    private void VerifyPublishedEvent()
    {
        var publishedEvents = _eventPublisher.OfType<TicketCreated>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().OrderId.Should().Be(OrderId);
    }

    private async Task VerifyTicketPersisted(Guid orderId)
    {
        var ticketPersisted = await _repository.Get(orderId);
        ticketPersisted.Should().NotBeNull();
        ticketPersisted.OrderId.Should().Be(OrderId);
        ticketPersisted.Status.Should().Be(TicketStatuses.WaitingApproval);
    }
}