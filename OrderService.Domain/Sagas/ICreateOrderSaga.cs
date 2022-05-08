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

    public async Task<SagaResult> Start(Guid orderId)
    {
        SagaDefinition = 
            WithState(new CreateOrderSagaState(orderId))
                .Step(_consumerService.ValidateOrderByConsumer)
                .Step(_kitchenService.CreateTicket)
                .Step(_accountingService.Authorize)
                .Step(_kitchenService.ConfirmTicket)
                .Step(_orderService.Approve)
            .Build();

        await Execute();
        
        return new SagaResult(true);
    }
}

public record SagaResult(bool Succeed);