using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Controllers
{
    public class CategoriesController : Controller
    {
        # region Services Handling
        private readonly IMovieRepository? _movieRepository;
        private readonly ICategoriesRepository? _categoriesRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITicketsCartRepository _ticketsCartRepository;

        public CategoriesController(IMovieRepository movieRepository,ICategoriesRepository categoriesRepository, UserManager<ApplicationUser> userManager, ITicketsCartRepository ticketsCartRepository) 
           {
            _movieRepository = movieRepository;
            _categoriesRepository = categoriesRepository;
            _userManager = userManager;
            _ticketsCartRepository = ticketsCartRepository;
        }
        #endregion
        #region Show Movies for A specific Category
        public IActionResult Category(string Category)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _ticketsCartRepository.Get(e => e.UserId == userId && e.TicketsStatus == Data.TicketsStatus.Shopping, c => c.TCinema).ToList();
                ViewData["Tickets"] = result;
                ViewData["TotalTickets"] = result?.Sum(e => e.TCinema?.Sum(t => t.QNumber) ?? 0) ?? 0;
            }
            ViewData["category"] = Category;
            return View(_movieRepository?.Get(cat => cat.Categories.Name == Category,
                Cinema => Cinema.Cinemas).ToList());
        }
        #endregion
        #region Add New Category
        [Authorize(Roles = "Admin")]
        public IActionResult AddCategory() 
        {
            ViewData["Show"] = false;
            return View(); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult AddCategory(Categories categories) 
        {
            if (ModelState.IsValid)
            {
                if(_categoriesRepository?.Get(cat => cat.Name == categories.Name).Any() == false)
                {
                    _categoriesRepository?.CreateNew(categories);
                    _categoriesRepository?.Commit();
                    return RedirectToAction("Index","Home");
                }
                TempData["Category Exists"] = categories.Name;
            }
			return View(categories);
		}
        #endregion
    }
}
