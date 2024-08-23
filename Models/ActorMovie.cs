namespace ETickets.Models
{
    public class ActorMovie
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        public Movie Movie { get; set; } = null!;
        public Actor Actor { get; set; } = null!;
    }
}
