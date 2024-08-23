namespace ETickets.Data
{
    public enum TicketsStatus
    {
        Shopping,    // The user is still shopping for tickets
        Pending,     // The user has paid, but the purchase is not yet confirmed
        Canceled,    // The ticket purchase has been canceled
        Confirmed    // The ticket purchase has been confirmed
    }
}
