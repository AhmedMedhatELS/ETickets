using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ETickets.Models.ViewModel
{
    public class TicketNumber
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITicketsCartRepository ticketsCartRepository;

        public TicketNumber(UserManager<ApplicationUser> userManager, ITicketsCartRepository ticketsCartRepository)
        {
            this.userManager = userManager;
            this.ticketsCartRepository = ticketsCartRepository;
        }

        public int GetNumberTickets(ClaimsPrincipal User)
        {
            var userId = userManager.GetUserId(User);
            var tickets = ticketsCartRepository.Get(t => t.UserId == userId && t.TicketsStatus == ETickets.Data.TicketsStatus.Shopping).ToList();
            return tickets?.Sum(c => c.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
        }
    }
}
