using System.ComponentModel.DataAnnotations.Schema;

namespace ETickets.Models
{
    public class TicketsCinema
    {
        public int TicketsCinemaId {  get; set; } 
        public int QNumber { get; set; }
        public int TicketsCartId { get; set; }
        [ForeignKey("TicketsCartId")]
        public TicketsCart? TicketsCart { get; set; }

        public int CinemaId { get; set; }
        [ForeignKey("CinemaId")]
        public Cinema? Cinema { get; set; }

    }
}
