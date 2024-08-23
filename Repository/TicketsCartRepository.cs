using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;

namespace ETickets.Repository
{
    public class TicketsCartRepository : Repository<TicketsCart>, ITicketsCartRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketsCartRepository(ApplicationDbContext context) : base(context) =>
            _context = context;

    }
}
