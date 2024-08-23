using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
namespace ETickets.Repository
{
    public class ActorRepository : Repository<Actor>,IActorRepository
    {
        private readonly ApplicationDbContext _context;
        public ActorRepository(ApplicationDbContext context) : base(context) =>
            _context = context;
    }
}
