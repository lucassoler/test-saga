using OrderService.Domain.Sagas;

namespace OrderService.Domain.Services;

public interface IKitchenServiceProxy
{
    Task CreateTicket(CreateOrderSagaState state);
    Task ConfirmTicket(CreateOrderSagaState state);
    Task CancelTicket(CreateOrderSagaState state);
}