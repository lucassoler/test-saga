using OrderService.Domain.Sagas;

namespace OrderService.Domain.Services;

public interface IOrderServiceProxy
{
    Task Approve(CreateOrderSagaState createOrderSagaState);
}