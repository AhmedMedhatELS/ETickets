using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
namespace ETickets.Repository
{
    public class CinemaMovieRepository : Repository<CinemaMovie>, ICinemaMovieRepository
    {
        private readonly ApplicationDbContext _context;
        public CinemaMovieRepository(ApplicationDbContext context) : base(context) =>
            _context = context;

    }
}
