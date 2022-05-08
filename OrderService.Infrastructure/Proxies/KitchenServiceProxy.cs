using OrderService.Domain.Sagas;
using OrderService.Domain.Services;

namespace OrderService.Infrastructure.Proxies;

public class KitchenServiceProxy : IKitchenServiceProxy
{
    public Task CreateTicket(CreateOrderSagaState createOrderSagaState)
    {
        throw new NotImplementedException();
    }

    public Task ConfirmTicket(CreateOrderSagaState createOrderSagaState)
    {
        throw new NotImplementedException();
    }
}