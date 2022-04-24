using OrderService.Domain;
using OrderService.Domain.Events;
using OrderService.Domain.Repositories;
using OrderService.Domain.Services;
using SharedKernel;

namespace OrderService.Application.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly ICreateOrderSaga _createOrderSaga;
    private readonly IDomainEventPublisher _eventPublisher;

    public CreateOrderCommandHandler(IOrderRepository repository,
        ICreateOrderSaga createOrderSaga,
        IDomainEventPublisher eventPublisher)
    {
        _repository = repository;
        _createOrderSaga = createOrderSaga;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(CreateOrderCommand command)
    {
        var order = new Order(command.OrderId);
        await _repository.Save(order);
        var result = await _createOrderSaga.Start(command.OrderId);
        
        if (result.Succeed)
        {
            await _eventPublisher.Publish(new OrderCreated(command.OrderId));
        }
    }
}

public record CreateOrderCommand(Guid OrderId) : ICommand;