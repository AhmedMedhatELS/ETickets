using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _context;
        public MovieRepository(ApplicationDbContext context) : base(context) =>
            _context = context;

    }
}
