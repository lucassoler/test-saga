using OrderService.Domain;
using OrderService.Domain.Events;
using OrderService.Domain.Repositories;
using SharedKernel;

namespace OrderService.Application.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly IDomainEventPublisher _eventPublisher;

    public CreateOrderCommandHandler(IOrderRepository repository,
        IDomainEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(CreateOrderCommand command)
    {
        var order = new Order(command.OrderId);
        await _repository.Save(order);
        await _eventPublisher.Publish(new OrderCreated(command.OrderId));
    }
}

public record CreateOrderCommand(Guid OrderId) : ICommand;