using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using OrderService.Application.Commands;
using OrderService.Domain.Events;
using OrderService.Infrastructure.Repositories;
using SharedKernel;
using Xunit;

namespace OrderService.Tests.Unit;

public class CreateOrderTests
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private readonly OrderRepositoryInMemory _repository;
    private readonly CreateOrderCommandHandler _handler;
    private readonly DomainEventPublisherInMemory _eventPublisher;

    public CreateOrderTests()
    {
        _repository = new OrderRepositoryInMemory();
        _eventPublisher = new DomainEventPublisherInMemory();
        _handler = new CreateOrderCommandHandler(_repository, _eventPublisher);
    }

    [Fact]
    public async Task CreateAnOrderShouldPersistAnOrderInPending()
    {
        await _handler.HandleAsync(CreateCommand());
        await VerifyPersistedOrder();
    }

    [Fact]
    public async Task CreateAnOrderShouldPublishOrderCreatedEvent()
    {
        await _handler.HandleAsync(CreateCommand());
        VerifyPublishedEvent();
    }

    private void VerifyPublishedEvent()
    {
        var publishedEvents = _eventPublisher.OfType<OrderCreated>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().OrderId.Should().Be(OrderId);
    }

    private static CreateOrderCommand CreateCommand()
    {
        return new CreateOrderCommand(OrderId);
    }

    private async Task VerifyPersistedOrder()
    {
        var orderPersisted = await _repository.Get(OrderId);
        orderPersisted.Should().NotBeNull();
        orderPersisted.OrderId.Should().Be(OrderId);
    }
}