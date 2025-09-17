using Microsoft.AspNetCore.Mvc;
using MyCeima.ViewModel;
using System.Threading.Tasks;

namespace MyCeima.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class MoviesController : Controller
    {
        ApplicationDBContext _Context;
        IRepository<Movies> _Movies;
        IRepository<Cinema> _Ceima;
        IRepository<Category> _Category;
        IRepository<Actor> _Actor;
        IRepository<ActorMovie> _ActorMovie;

        public MoviesController(IRepository<Movies> Movies, IRepository<Cinema> Ceima, IRepository<Category> category, IRepository<Actor> actor, IRepository<ActorMovie> actorMovie, ApplicationDBContext context)
        {
            _Movies = Movies;
            _Ceima = Ceima;
            _Category = category;
            _Actor = actor;
            _ActorMovie = actorMovie;
            _Context = context;
        }
        public async Task<IActionResult> IndexMovies()
        {
    

            var movies = await _Movies.GetAllAsync(Includes: [e => e.Category, e => e.Cinema]);

            return View(movies);
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {

          

            var movie = await _Movies.GetAllAsync();
            var category = await _Category.GetAllAsync();
            var Cinema = await _Ceima.GetAllAsync();
            var Actor = await _Actor.GetAllAsync();

            Movies mov = new Movies();

            Collection collection = new Collection()
            {
                Movies = movie,
                Categories = category,
                Cinemas = Cinema,
                Actors = Actor,
                JustMovie = mov
            };

            return View(collection);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movies movies, List<int> actors, IFormFile ImgUrl)
        {
            if (ModelState.IsValid)
            {
                return View(movies);
            }

           
          

            var filename = Guid.NewGuid() + Path.GetExtension(ImgUrl.FileName).ToLower();
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", filename);

                    using (var stream = System.IO.File.Create(filepath))
                    {
                        ImgUrl.CopyTo(stream);
                    }

            movies.ImgUrl = filename;

            await _Movies.CreateAsync(movies);
            await _Movies.Commit();

            foreach (var item in actors)
                {
                    await _ActorMovie.CreateAsync(new() { ActorId = item, MoviesId = movies.Id });
                    await _ActorMovie.Commit();
                }

            TempData["Success-Message"] = " Created Successfully";

            return RedirectToAction("IndexMovies");
        }


        public async Task<IActionResult> Remove([FromRoute]int id)
        {
            var movie = await _Movies.GetOne(Expression: e=>e.Id == id);

            if(movie == null)           
                return RedirectToAction("IndexMovies");
            

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",movie.ImgUrl);

            if(System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
        

            _Movies.Delete(movie);
            await _Movies.Commit();
            TempData["Success-Message"] = " Deleted Successfully Successfully";
            return RedirectToAction("IndexMovies");
        }


        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {


            var movie = await _Movies.GetOne(Expression: e => e.Id == id, Includes: [e=>e.Category, e=>e.Cinema, e => e.ActorMovie]);
            var Actormovie = await _ActorMovie.GetAllAsync(Includes: [e => e.Actor], Expression: e => e.MoviesId == id);
            var category = await _Category.GetAllAsync();
            var Cinema = await _Ceima.GetAllAsync();
            var Actor = await _Actor.GetAllAsync();

            SingleCollection collection = new SingleCollection()
            {
                Movies = movie,
                Categories = category,
                ActorMovie = Actormovie,
                Cinemas = Cinema,
                Actors = Actor
            };

            return View(collection);


        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movies movie, List<int> actors, IFormFile ImgUrl)
        {

            var mov = await _Movies.GetOne(Expression: e => e.Id == movie.Id, asNoTracking: true);

            if (actors is not null)
            {
                var actress = await _ActorMovie.GetAllAsync(Includes: [e => e.Actor], Expression: e => e.MoviesId == movie.Id);

                foreach (var item in actress)
                {
                    _ActorMovie.Delete(item);
                }
            }



            if (ImgUrl is not  null && ImgUrl.Length > 0)
            {
                         
                var fileName = Guid.NewGuid() + Path.GetExtension(ImgUrl.FileName);
                var newPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",fileName);
                using (var stream = System.IO.File.Create(newPath))
                {
                    ImgUrl.CopyTo(stream);
                }
                movie.ImgUrl = fileName;


                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", mov.ImgUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            else
            {
                movie.ImgUrl = mov.ImgUrl;
            }
           
            foreach (var item in actors)
            {
                _ActorMovie.Update(new() { ActorId = item, MoviesId = movie.Id });
                await _ActorMovie.Commit();
            }

            _Movies.Update(movie);
            await _Movies.Commit();

            TempData["Success-Message"] = " Updated Successfully Successfully";


            return RedirectToAction("IndexMovies");
        }



    }
}
