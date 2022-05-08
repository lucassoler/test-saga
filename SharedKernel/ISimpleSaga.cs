namespace SharedKernel;

public abstract class SimpleSaga<T>
{
    private T _state = default!;
    private readonly List<SagaStep<T>> _steps = new();
    protected SagaDefinition<T>? SagaDefinition;
    
    public SagaDefinition<T> Build()
    {
        return new SagaDefinition<T>(_steps);
    }


    protected SimpleSaga<T> WithState(T state)
    {
        _state = state;
        return this;
    }

    private SimpleSaga<T> Step(SagaStep<T> stepToExecute)
    {
        _steps.Add(stepToExecute);
        return this;
    }

    public SimpleSaga<T> Step(Func<T, Task> stepToExecute)
    {
        return Step(new SagaStep<T>(stepToExecute));
    }

    protected async Task Execute()
    {
        if (SagaDefinition is null)
        {
            return;
        }
        
        foreach (var step in SagaDefinition.SagaSteps)
        {
            await step.Invoke(_state);
        }
    }
}

public class SagaStep<T>
{
    private readonly Func<T, Task> _toExecute;
    public SagaStep(Func<T, Task> toExecute)
    {
        _toExecute = toExecute;
    }

    public async Task Invoke(T state)
    {
        await _toExecute(state);
    }
}