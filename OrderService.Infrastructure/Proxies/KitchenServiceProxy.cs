using OrderService.Domain.Sagas;
using OrderService.Domain.Services;

namespace OrderService.Infrastructure.Proxies;

public class KitchenServiceProxy : IKitchenServiceProxy
{
    public Task CreateTicket(CreateOrderSagaState state)
    {
        throw new NotImplementedException();
    }

    public Task ConfirmTicket(CreateOrderSagaState state)
    {
        throw new NotImplementedException();
    }

    public Task CancelTicket(CreateOrderSagaState state)
    {
        throw new NotImplementedException();
    }
}