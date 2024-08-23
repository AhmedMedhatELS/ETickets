namespace ETickets.Models
{
    public class CinemaMovie
    {
        public int MovieId { get; set; }
        public int CinemaId { get; set; }
        public Movie Movie { get; set; } = null!;
        public Cinema Cinema { get; set; } = null!;
    }
}
