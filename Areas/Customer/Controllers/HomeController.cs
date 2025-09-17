using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCeima.DataAccess;
using MyCeima.Utility;
using MyCeima.ViewModel;
using System.Threading.Tasks;

namespace MyCeima.Areas.Customer.Controllers
{
    [Area(SD.CustomerArea)]
    public class HomeController : Controller
    {

        private IRepository<Movies> _MovieRepo;
        private IRepository<Category> _Category;
        private IRepository<Cinema> _Ceima;
        private IRepository<ActorMovie> _ActorMovies;
        private IRepository<Actor> _Actor; 
        public HomeController(IRepository<Movies> MovieRepo , IRepository<Category> Category , IRepository<Cinema> Cima , IRepository<ActorMovie> actormovie , IRepository<Actor> actor)
        {
            _MovieRepo = MovieRepo;
            _Category = Category;
            _Ceima = Cima;
            _ActorMovies = actormovie;
            _Actor = actor;

        }

        public async Task<IActionResult> Index(Special? input , [FromQuery]int page = 1)
        {
            var movies = (await _MovieRepo.GetAllAsync(Includes: [e => e.Category , e=>e.Cinema])).AsQueryable();


            var Cinema = await _Ceima.GetAllAsync();
            ViewBag.Cima = Cinema;
            var Category = await _Category.GetAllAsync();
            ViewBag.Category = Category;


            if (input.NameMV is not null)
            {
                movies = movies.Where(e => e.Name.Contains(input.NameMV));
                ViewBag.Name = input.NameMV;
            }
              
            if (input.CategoryId is not null)

                movies = movies.Where(e => e.CategoryId == input.CategoryId);

            if (input.CinemaId is not null)
                movies = movies.Where(e => e.CinemaId == input.CinemaId);

            if (input.MovieStatus is not null)
                movies = movies.Where(e => e.MovieStatus == input.MovieStatus);


            var mymoveis = Math.Ceiling(movies.Count() / 3.5);
           
            movies = movies.Skip((page - 1) * 4).Take(4);


            ViewBag.Move = mymoveis;
            ViewBag.Current = page;

            return View(movies.ToList());
        }



        public async Task<IActionResult> Category()
        {
            var Category = await _Category.GetAllAsync();

            return View(Category);
        }


        public async Task<IActionResult> Ceima()
        {
            var cima = await _Ceima.GetAllAsync();

            return View(cima);
        }

        public async Task<IActionResult> Details([FromRoute] int id)
        {


            var mov = await _MovieRepo.GetOne(Includes: [e => e.Category, e => e.Cinema, e => e.ActorMovie], Expression: e => e.Id == id);
            


            var product = await _ActorMovies.GetAllAsync(Includes: [e =>e.Actor], Expression: e => e.MoviesId == id);           
            
            var justActor = new Actor();

            NewClass Indded = new()
            {
                myMov = mov,
                Act = product,
                actor = justActor

            };

            return View(Indded);
        }

        
        public async Task<IActionResult> ActorDetails([FromRoute] int id)
        {
            var actor = await _Actor.GetOne(Expression: e => e.Id == id);

            var product = await _ActorMovies.GetAllAsync(Includes: [e=>e.Movie], Expression: e => e.ActorId == id);
           
            var mov = new Movies();

            SecondClass Second = new() 
            {
                Actor = actor,
                Movies = product,
                movie = mov
            };

            return View(Second);
        }

        public async Task<IActionResult> CategoryCollection([FromRoute] int id)
        {
            var movies = await _MovieRepo.GetAllAsync(Includes: [e=>e.Cinema , e=>e.Category] , Expression: e=>e.CategoryId == id);


            var onemovie = await _Category.GetOne(Expression: e => e.Id == id);          
            
            ViewBag.CatID = onemovie;

          


            return View(movies);
        }

        public async Task<IActionResult> CenimaCollection([FromRoute] int id)
        {
            var movies = await  _MovieRepo.GetAllAsync(Includes: [e => e.Cinema, e => e.Category], Expression: e=>e.CinemaId == id);



            var onemovie = await _Ceima.GetOne(Expression: e => e.Id == id);
            
            ViewBag.CatID = onemovie;


            return View(movies.ToList());
        }

    }


}
