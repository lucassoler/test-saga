using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
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
    private readonly IConsumerServiceProxy _consumerProxy = Substitute.For<IConsumerServiceProxy>();
    private readonly IKitchenServiceProxy _kitchenProxy = Substitute.For<IKitchenServiceProxy>();
    private readonly IAccountingServiceProxy _accountingProxy = Substitute.For<IAccountingServiceProxy>();
    private readonly IOrderServiceProxy _orderProxy = Substitute.For<IOrderServiceProxy>();

    public CreateOrderTests()
    {
        _repository = new OrderRepositoryInMemory();
        _eventPublisher = new DomainEventPublisherInMemory();
        var createOrderSaga = new CreateOrderSaga(
            _kitchenProxy,
            _consumerProxy,
            _accountingProxy,
            _orderProxy
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
        await _consumerProxy.Received().ValidateOrderByConsumer(new CreateOrderSagaState(OrderId));
    }

    [Fact]
    public async Task CreateAnOrderShouldCreateTicketInKitchen()
    {
        await _handler.HandleAsync(CreateCommand());
        await _kitchenProxy.Received().CreateTicket(new CreateOrderSagaState(OrderId));
    }

    [Fact]
    public async Task CreateAnOrderShouldAuthorizePaymentInAccounting()
    {
        await _handler.HandleAsync(CreateCommand());
        await _accountingProxy.Received().Authorize(new CreateOrderSagaState(OrderId));
    }

    [Fact]
    public async Task AuthorizationFromAccountingShouldConfirmTicketInKitchen()
    {
        await _handler.HandleAsync(CreateCommand());
        await _kitchenProxy.Received().ConfirmTicket(new CreateOrderSagaState(OrderId));
    }

    [Fact]
    public async Task OrderShouldBeApprovedAfterTheTicketHasBeenConfirmedInKitchen()
    {
        await _handler.HandleAsync(CreateCommand());
        await _orderProxy.Received().Approve(new CreateOrderSagaState(OrderId));
    }

    [Fact]
    public async Task CreateOrderShouldRevokeOrderCreationIfValidatingConsumerFailed()
    {
        _consumerProxy.ValidateOrderByConsumer(Arg.Any<CreateOrderSagaState>()).ThrowsForAnyArgs(new Exception());
        await _handler.HandleAsync(CreateCommand());
        await _orderProxy.Received().Reject(new CreateOrderSagaState(OrderId));
    }

    [Fact]
    public async Task CreateOrderShouldNotCancelTicketIfValidatingConsumerFailed()
    {
        _consumerProxy.ValidateOrderByConsumer(Arg.Any<CreateOrderSagaState>()).ThrowsForAnyArgs(new Exception());
        await _handler.HandleAsync(CreateCommand());
        await _kitchenProxy.DidNotReceiveWithAnyArgs().CancelTicket(Arg.Any<CreateOrderSagaState>());
    }


    [Fact]
    public async Task CreateOrderShouldCancelTicketIfAuthorizingConsumerFailed()
    {
        _accountingProxy.Authorize(Arg.Any<CreateOrderSagaState>()).ThrowsForAnyArgs(new Exception());
        await _handler.HandleAsync(CreateCommand());
        await _kitchenProxy.Received().CancelTicket(Arg.Any<CreateOrderSagaState>());
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