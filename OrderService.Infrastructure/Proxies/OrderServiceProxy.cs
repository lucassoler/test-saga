using OrderService.Domain.Sagas;
using OrderService.Domain.Services;

namespace OrderService.Infrastructure.Proxies;

public class OrderServiceProxy : IOrderServiceProxy
{
    public Task Approve(CreateOrderSagaState createOrderSagaState)
    {
        throw new NotImplementedException();
    }

    public Task Reject(CreateOrderSagaState createOrderSagaState)
    {
        throw new NotImplementedException();
    }
}