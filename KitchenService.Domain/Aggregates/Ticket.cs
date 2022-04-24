namespace KitchenService.Application.Commands;

public class Ticket
{
    private readonly Guid _orderId;

    public Ticket(Guid orderId)
    {
        _orderId = orderId;
    }
}