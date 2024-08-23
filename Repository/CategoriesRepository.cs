using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace ETickets.Repository
{
    public class CategoriesRepository : Repository<Categories>, ICategoriesRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoriesRepository(ApplicationDbContext context) : base(context) =>
            _context = context;
    }
}
