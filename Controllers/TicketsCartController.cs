using ETickets.Models;
using ETickets.Models.ViewModel;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Stripe.Checkout;
using System.Linq.Expressions;
using System.Net.Sockets;

namespace ETickets.Controllers
{
    public class TicketsCartController : Controller
    {
        #region Services Handling
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketsCartRepository _ticketRepository;
        private readonly ITicketsCinemaRepository _cinemaRepository;

        public TicketsCartController(UserManager<ApplicationUser> userManager, ITicketsCartRepository ticketRepository, ITicketsCinemaRepository cinemaRepository)
        {
            _userManager = userManager;
            _ticketRepository = ticketRepository;
            _cinemaRepository = cinemaRepository;
        }
        #endregion
        #region Cart Page
        [Authorize]
        public IActionResult Cart()
        {
            var userId = _userManager.GetUserId(User);
            if(userId != null)
            {
                var movies = _ticketRepository.Get(
                    m => m.UserId == userId && m.TicketsStatus == Data.TicketsStatus.Shopping
                    , tc => tc.TCinema
                    , mov => mov.Movie
                    , movc => movc.Movie.Cinemas
                    ).ToList();
                ViewData["TotalTickets"] = movies?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
                return View(movies);
            }
            return View(new List<TicketsCart>());
        }
        #endregion
        #region Add and Edit Tickets In Cart
        [Authorize]
        public IActionResult BookTicket(TicketCartVM ticketCartVM)
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                var userTickets = _ticketRepository.Get(e => (e.UserId == userId && e.MovieId == ticketCartVM.MovieId && e.TicketsStatus == Data.TicketsStatus.Shopping), c => c.TCinema).FirstOrDefault();
                
                if (userTickets == null) 
                {
                    if(ticketCartVM.QNumber != 0)
                    {
                        TicketsCart ticketsCart = new()
                        {
                            MovieId = ticketCartVM.MovieId,
                            UserId = userId,
                            TicketsStatus = Data.TicketsStatus.Shopping
                        };

                        _ticketRepository.CreateNew(ticketsCart);
                        _ticketRepository.Commit();
                        
                        var ticket = _ticketRepository.Get(e => e == ticketsCart).FirstOrDefault();
                        
                        if (ticket != null)
                        {
                            TicketsCinema ticketsCinema = new()
                            {
                                QNumber = ticketCartVM.QNumber,
                                TicketsCartId = ticket.TicketsCartId,
                                CinemaId = ticketCartVM.CinemaId
                            };
                            _cinemaRepository.CreateNew(ticketsCinema);
                            _cinemaRepository.Commit();
                        }
                    }
                }
                else
                {
                    var usercinema = _cinemaRepository.Get(e => e.TicketsCartId == userTickets.TicketsCartId && e.CinemaId == ticketCartVM.CinemaId).FirstOrDefault();
                    
                    if (usercinema == null)
                    {
                        if (ticketCartVM.QNumber != 0)
                        {
                            TicketsCinema ticketCinema = new()
                            {
                                QNumber = ticketCartVM.QNumber,
                                CinemaId= ticketCartVM.CinemaId,
                                TicketsCartId = userTickets.TicketsCartId
                            };
                            _cinemaRepository.CreateNew(ticketCinema);
                            _cinemaRepository.Commit();
                        }
                    }
                    else
                    {
                        if(ticketCartVM.QNumber != 0)
                        {
                            usercinema.QNumber = ticketCartVM.QNumber;
                            _cinemaRepository.Edit(usercinema);
                            _cinemaRepository.Commit();
                        }
                        else
                        {
                            _cinemaRepository.Delete(usercinema);
                            _cinemaRepository.Commit();

                            if (userTickets.TCinema?.Count == 0) 
                            {
                                _ticketRepository.Delete(userTickets);
                                _ticketRepository.Commit();
                            }
                        }
                    }
                }
            }
            if (ticketCartVM.ViewNavigation == Data.ViewNavigation.HomeIndex)
                return RedirectToAction("Index", "Home");
            else if (ticketCartVM.ViewNavigation == Data.ViewNavigation.MovieDetails)
                return RedirectToAction("Index", "Movie", new { id = ticketCartVM.MovieId });
            else if (ticketCartVM.ViewNavigation == Data.ViewNavigation.TicketCart)
                return RedirectToAction("Cart", "TicketsCart");
            else 
                return NotFound();

        }
        #endregion
        #region Delete Movie From Cart
        [Authorize]
        public IActionResult Delete(int TicketsCartId)
        {
            var Tmovie = _ticketRepository.Get(tm => tm.TicketsCartId == TicketsCartId, tc => tc.TCinema).FirstOrDefault();
            if (Tmovie != null)
            {
                foreach (var TCinema in Tmovie.TCinema)
                    _cinemaRepository.Delete(TCinema);
                _cinemaRepository.Commit();

                _ticketRepository.Delete(Tmovie);
                _ticketRepository.Commit();
            }
            
            return RedirectToAction("Cart", "TicketsCart");
        }
        #endregion
        #region Pay
        public IActionResult pay()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            var userId = _userManager.GetUserId(User);
            var items = _ticketRepository.Get(t => t.UserId == userId && t.TicketsStatus == Data.TicketsStatus.Shopping, m => m.Movie, Tc => Tc.TCinema);
           
            foreach (var model in items)
            {
                var result = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = model.Movie.Title,
                        },
                        UnitAmount = (long)Math.Round(model.Movie.Price * 100),
                    },
                    Quantity = model.TCinema.Sum(t => t.QNumber),
                };
                options.LineItems.Add(result);
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);

        }
        #endregion
        #region Orders List
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            #region Variables
            var userId = _userManager.GetUserId(User);

            var arrangeOrders = new List<OrdersArrangeVM>();

            var orders = new List<TicketsCart>();
            #endregion
            #region Show Tickets Number
            if (userId != null)
            {
                var movies = _ticketRepository.Get(
                    m => m.UserId == userId && m.TicketsStatus == Data.TicketsStatus.Shopping
                    , tc => tc.TCinema
                    ).ToList();
                ViewData["TotalTickets"] = movies?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            #endregion
            #region Admin Or User
            if (User.IsInRole("Admin"))            
                orders = _ticketRepository.Get(s =>
                                        s.TicketsStatus != Data.TicketsStatus.Shopping, 
                                        t => t.TCinema, 
                                        mbox => mbox.Movie).ToList();                                            
            else                          
                orders = _ticketRepository.Get(s =>
                                        s.TicketsStatus != Data.TicketsStatus.Shopping &&
                                        s.UserId == userId,
                                        t => t.TCinema,
                                        m => m.Movie).ToList();
            #endregion
            #region Managing List Orders 
            foreach (var item in orders)
            {
                decimal price = item.TCinema.Sum(t => t.QNumber) * item.Movie.Price;

                var existingOrder = arrangeOrders.FirstOrDefault(e => e.OrderId == item.PaymentId);

                if (existingOrder != null)
                    existingOrder.TotalPrice += price;
                else
                {
                    var user = await _userManager.FindByIdAsync(item.UserId);
                    string name = user?.UserName ?? "NotFound";

                    var newOrder = new OrdersArrangeVM
                    {
                        OrderId = item.PaymentId ?? "",
                        TotalPrice = price,
                        OrderName = name,
                        Status = item.TicketsStatus
                    };

                    arrangeOrders.Add(newOrder);
                }
            }
            #endregion
            return View(arrangeOrders);
        }
        #endregion
        #region confirm Order
        public IActionResult ConfirmOrder(string  orderId)
        {
            if (orderId == null) return NotFound();
            var orders = _ticketRepository.Get(o => o.PaymentId ==  orderId).ToList();
            foreach (var item in orders)
            {
                item.TicketsStatus = Data.TicketsStatus.Confirmed;
                _ticketRepository.Edit(item);
            }
            _ticketRepository.Commit();
            return RedirectToAction("Orders", "TicketsCart");
        }
        #endregion
        #region Cancel Order
        public IActionResult CancelOrder(string orderId)
        {
            if (orderId == null) return NotFound();
            var orders = _ticketRepository.Get(o => o.PaymentId == orderId).ToList();
            foreach (var item in orders)
            {
                item.TicketsStatus = Data.TicketsStatus.Canceled;
                _ticketRepository.Edit(item);
            }
            _ticketRepository.Commit();
            return RedirectToAction("Orders", "TicketsCart");
        }
        #endregion

    }
}
