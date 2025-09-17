using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MyCeima.Areas.Admin.Controllers
{

    [Area(SD.AdminArea)]
    public class HomeController : Controller
    {
        IRepository<Movies> _Movies;      
        IRepository<Category> _Category;      
        IRepository<Cinema> _Cinema;      
        IRepository<Actor> _Actor;  
        

        public HomeController(IRepository<Movies> Movies , IRepository<Category> Category , IRepository<Cinema> Cinema, IRepository<Actor> Actor)
        {
            _Movies = Movies;
            _Category = Category;
            _Actor = Actor;
            _Cinema = Cinema;
        }

        [Authorize(Roles =$"{SD.SuperAdmin},{SD.Admin}")]
        public async Task<IActionResult> Index()
        {

            var movie = await _Movies.GetAllAsync();
            var category = await _Category.GetAllAsync();
            var Cinema = await _Cinema.GetAllAsync();
            var Actor = await _Actor.GetAllAsync();

            Collection collection = new Collection()
            {
                Movies = movie,
                Categories = category,
                Cinemas = Cinema,
                Actors = Actor
            };

            return View(collection);
        }



    }
}
