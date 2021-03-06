namespace SharedKernel;


public interface ICommandDispatcher  
{  
    Task HandleAsync<T>(T command) where T:ICommand;  
} 