using OrderService.Domain.Sagas;

namespace OrderService.Domain.Services;

public interface IAccountingServiceProxy
{
    public Task Authorize(CreateOrderSagaState createOrderSagaState);
}