using ETickets.Data;

namespace ETickets.Models.ViewModel
{
    public class TicketCartVM
    {
        public int MovieId { get; set; }
        public int CinemaId { get; set; }
        public int QNumber {  get; set; }  
        public ViewNavigation ViewNavigation { get; set; }
    }
}
