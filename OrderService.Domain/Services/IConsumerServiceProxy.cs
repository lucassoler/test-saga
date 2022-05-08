using OrderService.Domain.Sagas;

namespace OrderService.Domain.Services;

public interface IConsumerServiceProxy
{
    Task ValidateOrderByConsumer(CreateOrderSagaState createOrderSagaState);
}