using OrderService.Domain.Sagas;
using OrderService.Domain.Services;

namespace OrderService.Infrastructure.Proxies;

public class AccountingServiceProxy : IAccountingServiceProxy
{
    public Task Authorize(CreateOrderSagaState createOrderSagaState)
    {
        throw new NotImplementedException();
    }
}