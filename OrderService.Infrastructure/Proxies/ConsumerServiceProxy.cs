using OrderService.Domain.Sagas;
using OrderService.Domain.Services;

namespace OrderService.Infrastructure.Proxies;

public class ConsumerServiceProxy : IConsumerServiceProxy
{
    public Task ValidateOrderByConsumer(CreateOrderSagaState createOrderSagaState)
    {
        throw new NotImplementedException();
    }
}