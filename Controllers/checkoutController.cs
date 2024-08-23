using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using SessionService = Stripe.BillingPortal.SessionService;

namespace ETickets.Controllers
{
    public class checkoutController : Controller
    {
        #region Services Handling
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketsCartRepository _ticketRepository;
       
        public checkoutController(UserManager<ApplicationUser> userManager,
            ITicketsCartRepository ticketRepository)
        {
            _userManager = userManager;
            _ticketRepository = ticketRepository;
        }
        #endregion
        public IActionResult success(string session_id)
        {
            var userId = _userManager.GetUserId(User);
            var userTickets = _ticketRepository.Get(t => t.UserId == userId && t.TicketsStatus == Data.TicketsStatus.Shopping).ToList();

            //var service = new SessionService();

            //// Use the `List` method as a workaround to get the session if Get isn't available
            //var options = new SessionListOptions
            //{
            //    Limit = 1,
            //    PaymentIntent = session_id,  // Filter by session ID if applicable
            //};

            //var sessions = service.List();

            //var session = sessions.Data.FirstOrDefault();

            //var paymentIntentId = session?.PaymentIntentId;


            foreach (var ticket in userTickets)
            {
                ticket.TicketsStatus = Data.TicketsStatus.Pending;
                ticket.PaymentId = session_id;
                _ticketRepository.Edit(ticket);
            }  
            _ticketRepository.Commit();

            return View();
        }
        public IActionResult cancel()
        {
            return View();
        }
    }
}
