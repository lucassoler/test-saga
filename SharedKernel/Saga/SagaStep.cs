namespace SharedKernel.Saga;

public class SagaStep<T>
{
    private Func<T, Task>? _toExecute;
    private Func<T, Task>? _compensation;

    public void WithCompensation(Func<T, Task> compensation)
    {
        _compensation = compensation;
    }

    public void InvokeParticipant(Func<T, Task> toExecute)
    {
        _toExecute = toExecute;
    }

    public async Task Invoke(T state)
    {
        if (_toExecute is not null)
        {
            await _toExecute(state);
        }
    }

    public async Task Revoke(T state)
    {
        if (_compensation is not null)
        {
            await _compensation(state);
        }
    }
}