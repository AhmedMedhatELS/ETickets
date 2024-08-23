using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;

namespace ETickets.Repository
{
    public class TicketsCinemaRepository : Repository<TicketsCinema>, ITicketsCinemaRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketsCinemaRepository(ApplicationDbContext context) : base(context) =>
            _context = context;

    }
}
