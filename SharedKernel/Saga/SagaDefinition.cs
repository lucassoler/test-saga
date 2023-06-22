namespace SharedKernel.Saga;

public class SagaDefinition<T>
{
    public readonly List<SagaStep<T>> SagaSteps;

    public SagaDefinition(List<SagaStep<T>> sagaSteps)
    {
        SagaSteps = sagaSteps;
    }
}