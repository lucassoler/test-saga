using OrderService.Domain;
using OrderService.Domain.Exceptions;
using OrderService.Domain.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepositoryInMemory : IOrderRepository
{    
    private readonly IList<Order> _ordersPersisted;

    public OrderRepositoryInMemory(List<Order>? orders = null)
    {
        _ordersPersisted = orders ?? new List<Order>();
    }

    public Task Save(Order order)
    {
        var index = _ordersPersisted.IndexOf(order);
        if (index == -1)
        {
            _ordersPersisted.Remove(order);
        }
        
        _ordersPersisted.Add(order);
        return Task.CompletedTask;
    }

    public Task<Order> Get(Guid orderId)
    
    {
        var result = _ordersPersisted.FirstOrDefault(x => x.OrderId == orderId);

        if (result is null)
        {
            throw new OrderNotFoundException();
        }
        
        return Task.FromResult(result);
    }
}