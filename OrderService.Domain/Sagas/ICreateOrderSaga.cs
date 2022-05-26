using OrderService.Domain.Services;
using SharedKernel;

namespace OrderService.Domain.Sagas;

public record CreateOrderSagaState(Guid OrderId);

public class CreateOrderSaga : SimpleSaga<CreateOrderSagaState>
{
    private readonly IKitchenServiceProxy _kitchenService;
    private readonly IConsumerServiceProxy _consumerService;
    private readonly IAccountingServiceProxy _accountingService;
    private readonly IOrderServiceProxy _orderService;

    public CreateOrderSaga(IKitchenServiceProxy kitchenService,
        IConsumerServiceProxy consumerService,
        IAccountingServiceProxy accountingService,
        IOrderServiceProxy orderService)
    {
        _kitchenService = kitchenService;
        _consumerService = consumerService;
        _accountingService = accountingService;
        _orderService = orderService;
    }

    public async Task Start(Guid orderId)
    {
        SagaDefinition = 
            WithState(new CreateOrderSagaState(orderId))
                .Step()
                    .WithCompensation(_orderService.Reject)
                .Step()
                    .InvokeParticipant(_consumerService.ValidateOrderByConsumer)
                .Step()
                    .InvokeParticipant(_kitchenService.CreateTicket)
                    .WithCompensation(_kitchenService.CancelTicket)
                .Step()
                    .InvokeParticipant(_accountingService.Authorize)
                .Step()
                    .InvokeParticipant(_kitchenService.ConfirmTicket)
                .Step()
                    .InvokeParticipant(_orderService.Approve)
            .Build();

        await Execute();
    }
}