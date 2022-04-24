namespace OrderService.Domain.Services;

public interface ICreateOrderSaga
{
    Task<SagaResult> Start(Guid orderId);
}

public record SagaResult(bool Succeed);