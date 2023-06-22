using SharedKernel.Saga;

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

    public SimpleSaga<T> Step()
    {
        _steps.Add(new SagaStep<T>());
        return this;
    }

    public SimpleSaga<T> InvokeParticipant(Func<T, Task> participantToExecute)
    {
        _steps.Last().InvokeParticipant(participantToExecute);
        return this;
    }

    public SimpleSaga<T> WithCompensation(Func<T, Task> compensation)
    {
        _steps.Last().WithCompensation(compensation);
        return this;
    }

    protected async Task Execute()
    {
        if (SagaDefinition is null || !SagaDefinition.SagaSteps.Any())
        {
            return;
        }

        var stepIndex = 0;
        var inError = false;
        await SagaDefinition.SagaSteps.ForEachAsync(async step =>
        {
            if (inError)
            {
                return;
            }
            
            try
            {
                await step.Invoke(_state);
                stepIndex++;
            }
            catch
            {
                inError = true;
            }
        });

        if (inError)
        {
            await RevokeSaga(stepIndex);
        }
    }

    private async Task RevokeSaga(int lastStepIndex)
    {
        if (SagaDefinition is null)
        {
            return;
        }

        await SagaDefinition.SagaSteps
            .GetRange(0, lastStepIndex)
            .ReverseList()
            .ForEachAsync(async t => await t.Revoke(_state));
    }
}