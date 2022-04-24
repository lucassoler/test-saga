namespace OrderService.Domain;

public class Order
{
    public readonly Guid OrderId;

    public Order(Guid orderId)
    {
        OrderId = orderId;
    }
}