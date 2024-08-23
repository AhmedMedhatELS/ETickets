using ETickets.Data;

namespace ETickets.Models.ViewModel
{
    public class OrdersArrangeVM
    {
        public string OrderId { get; set; } = null!;
        public string OrderName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public TicketsStatus Status { get; set; }

    }
}
