using OrderService.Domain.Sagas;

namespace OrderService.Domain.Services;

public interface IKitchenServiceProxy
{
    Task CreateTicket(CreateOrderSagaState createOrderSagaState);
    Task ConfirmTicket(CreateOrderSagaState createOrderSagaState);
}