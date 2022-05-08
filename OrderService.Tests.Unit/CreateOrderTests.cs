using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Events;
using OrderService.Domain.Sagas;
using OrderService.Domain.Services;
using OrderService.Infrastructure.Proxies;
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
    private readonly Mock<IConsumerServiceProxy> _consumerProxy;
    private readonly Mock<IKitchenServiceProxy> _kitchenProxy;
    private readonly Mock<IAccountingServiceProxy> _accountingProxy;
    private readonly Mock<IOrderServiceProxy> _orderProxy;

    public CreateOrderTests()
    {
        _repository = new OrderRepositoryInMemory();
        _eventPublisher = new DomainEventPublisherInMemory();
        _consumerProxy = new Mock<IConsumerServiceProxy>();
        _kitchenProxy = new Mock<IKitchenServiceProxy>();
        _accountingProxy = new Mock<IAccountingServiceProxy>();
        _orderProxy = new Mock<IOrderServiceProxy>();
        var createOrderSaga = new CreateOrderSaga(
            _kitchenProxy.Object,
            _consumerProxy.Object,
            _accountingProxy.Object,
            _orderProxy.Object
        );
        _handler = new CreateOrderCommandHandler(_repository, createOrderSaga, _eventPublisher);
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
    public async Task CreateAnOrderShouldVerifyConsumer()
    {
        await _handler.HandleAsync(CreateCommand());
        _consumerProxy.Verify(x => x.ValidateOrderByConsumer(new CreateOrderSagaState(OrderId)), Times.Once);
    }

    [Fact]
    public async Task CreateAnOrderShouldCreateTicketInKitchen()
    {
        await _handler.HandleAsync(CreateCommand());
        _kitchenProxy.Verify(x => x.CreateTicket(new CreateOrderSagaState(OrderId)), Times.Once);
    }

    [Fact]
    public async Task CreateAnOrderShouldAuthorizePaymentInAccounting()
    {
        await _handler.HandleAsync(CreateCommand());
        _accountingProxy.Verify(x => x.Authorize(new CreateOrderSagaState(OrderId)), Times.Once);
    }

    [Fact]
    public async Task AuthorizationFromAccountingShouldConfirmTicketInKitchen()
    {
        await _handler.HandleAsync(CreateCommand());
        _kitchenProxy.Verify(x => x.ConfirmTicket(new CreateOrderSagaState(OrderId)), Times.Once);
    }

    [Fact]
    public async Task OrderShouldBeApprovedAfterTheTicketHasBeenConfirmedInKitchen()
    {
        await _handler.HandleAsync(CreateCommand());
        _orderProxy.Verify(x => x.Approve(new CreateOrderSagaState(OrderId)), Times.Once);
    }

    /*[Fact]
    public async Task CreateAnOrderShouldNotPublishOrderCreatedIfSagaFailed()
    {
        SetupSagaResult(new SagaResult(false));
        await _handler.HandleAsync(CreateCommand());
        VerifyEventNotPublished();
    }*/

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