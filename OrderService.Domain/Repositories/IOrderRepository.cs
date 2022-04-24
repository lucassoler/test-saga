namespace OrderService.Domain.Repositories;

public interface IOrderRepository
{
    Task Save(Order order);
    Task<Order> Get(Guid orderId);
}