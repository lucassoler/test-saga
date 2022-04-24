namespace KitchenService.Domain.Aggregates;

public class Ticket
{
    public readonly Guid OrderId;
    public TicketStatuses Status = TicketStatuses.WaitingApproval;

    public Ticket(Guid orderId)
    {
        OrderId = orderId;
    }

    public void Approve()
    {
        Status = TicketStatuses.Approved;
    }
}

public enum TicketStatuses
{
    WaitingApproval,
    Approved
}