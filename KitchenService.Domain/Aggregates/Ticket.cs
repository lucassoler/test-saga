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

    public void Reject()
    {
        Status = TicketStatuses.Rejected;
    }
}

public enum TicketStatuses
{
    WaitingApproval,
    Approved,
    Rejected
}