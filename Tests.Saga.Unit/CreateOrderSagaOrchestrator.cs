using Xunit;

namespace Tests.Unit;

public class CreateOrderSagaOrchestrator
{
    [Fact]
    public void CreateOrderShouldVerifyConsumer()
    {
        var consumerService = new ConsumerVerifier();
        var createOrderSaga = new CreateOrderSaga();
        createOrderSaga.Start();
    }
}

public class ConsumerVerifier
{
}

public class CreateOrderSaga
{
    public void Start()
    {
        throw new System.NotImplementedException();
    }
}