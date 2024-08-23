using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
namespace ETickets.Repository
{
    public class ActorMovieRepository : Repository<ActorMovie> ,IActorMovieRepository 
    {
        private readonly ApplicationDbContext _context;
        public ActorMovieRepository(ApplicationDbContext context) : base(context) =>
            _context = context;
        
    }
}
