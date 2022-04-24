namespace Contracts;

public class CommandDispatcher : ICommandDispatcher  
{  
    private readonly IServiceProvider _serviceProvider;  
  
    public CommandDispatcher(IServiceProvider serviceProvider)  
    {  
        _serviceProvider = serviceProvider;  
    }  
  
    public async Task HandleAsync<T>(T command) where T : ICommand  
    {  
        var service = _serviceProvider.GetService(typeof(ICommandHandler<T>)) as ICommandHandler<T>;  
        await service!.HandleAsync(command);  
    }  
}