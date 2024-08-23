using ETickets.Data;

namespace ETickets.Models
{
	public class SearchView
	{
		public List<Movie>? Movies { get; set; }
		public List<Cinema>? Cinemas { get; set; }
		public List<Actor>? Actors { get; set; }
	}
}
