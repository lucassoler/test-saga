using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Events;
using OrderService.Domain.Services;
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
    private readonly Mock<ICreateOrderSaga> _createOrderSaga;

    public CreateOrderTests()
    {
        _repository = new OrderRepositoryInMemory();
        _eventPublisher = new DomainEventPublisherInMemory();
        _createOrderSaga = new Mock<ICreateOrderSaga>();
        SetupSagaResult(new SagaResult(true));
        _handler = new CreateOrderCommandHandler(_repository, _createOrderSaga.Object, _eventPublisher);
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

    [Fact]
    public async Task CreateAnOrderShouldStartCreateOrderSaga()
    {
        await _handler.HandleAsync(CreateCommand());
        _createOrderSaga.Verify(x => x.Start(OrderId), Times.Once);
    }

    [Fact]
    public async Task CreateAnOrderShouldNotPublishOrderCreatedIfSagaFailed()
    {
        SetupSagaResult(new SagaResult(false));
        await _handler.HandleAsync(CreateCommand());
        VerifyEventNotPublished();
    }

    private void SetupSagaResult(SagaResult sagaResult)
    {
        _createOrderSaga.Setup(x => x.Start(It.IsAny<Guid>())).ReturnsAsync(sagaResult);
    }

    private void VerifyPublishedEvent()
    {
        var publishedEvents = _eventPublisher.OfType<OrderCreated>().ToList();
        publishedEvents.Count.Should().Be(1);
        publishedEvents.First().OrderId.Should().Be(OrderId);
    }

    private void VerifyEventNotPublished()
    {
        var publishedEvents = _eventPublisher.OfType<OrderCreated>().ToList();
        publishedEvents.Count.Should().Be(0);
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