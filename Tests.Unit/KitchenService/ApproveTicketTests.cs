using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using FluentAssertions;
using KitchenService.Application.Commands;
using KitchenService.Domain.Aggregates;
using KitchenService.Infrastructure.Repositories;
using Xunit;

namespace Tests.Consumer.Unit.KitchenService;

public class ApproveTicketTests
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private readonly ApproveTicketCommandHandler _handler;
    private readonly ApproveTicketCommand _command;
    private readonly DomainEventPublisherInMemory _eventPublisher;
    private readonly TicketRepositoryInMemory _repository;

    public ApproveTicketTests()
    {
        _eventPublisher = new DomainEventPublisherInMemory();
        _repository = new TicketRepositoryInMemory(new List<Ticket> { new(OrderId)});
        _handler = new ApproveTicketCommandHandler(_repository);
        _command = CreateCommand(OrderId);
    }

    [Fact]
    public async Task ApproveATicketShouldUpdateHisStatusToApproved()
    {
        await _handler.HandleAsync(_command);
        await VerifyTicketPersisted(OrderId);
    }

    private async Task VerifyTicketPersisted(Guid orderId)
    {
        var ticketPersisted = await _repository.Get(orderId);
        ticketPersisted.Should().NotBeNull();
        ticketPersisted.OrderId.Should().Be(OrderId);
        ticketPersisted.Status.Should().Be(TicketStatuses.Approved);
    }

    private static ApproveTicketCommand CreateCommand(Guid orderId)
    {
        return new ApproveTicketCommand(orderId);
    }
}