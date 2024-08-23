using ETickets.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETickets.Models
{
    public class TicketsCart
    {
        public int TicketsCartId { get; set; }
        public TicketsStatus TicketsStatus { get; set; }
        public string? PaymentId {  get; set; } 
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; } = null!;
        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public List<TicketsCinema> TCinema { get; set; } = [];

        public override bool Equals(object? obj) =>
                      obj is TicketsCart ticketsCart &&
                      this.MovieId == ticketsCart.MovieId &&
                      this.UserId == ticketsCart.UserId &&
                      this.TicketsStatus == ticketsCart.TicketsStatus;
        public override int GetHashCode() =>
        HashCode.Combine(MovieId, UserId, TicketsStatus);
    }
}
