namespace SharedKernel;

public interface ICommandHandler<in T> where T : ICommand
{
    Task HandleAsync(T command);
}