using Microsoft.AspNetCore.Mvc;
using MyCeima.ViewModel;

namespace MyCeima.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]

    public class ActorController : Controller
    {


        IRepository<Actor> _Actor;

        public ActorController(IRepository<Actor> actor)
        {
            _Actor = actor;
        }

        public async Task<IActionResult> ActorIndex()
        {
            var Actor = await _Actor.GetAllAsync();

            return View(Actor);

        }


        [HttpGet]
        public IActionResult CreateActor()
        {


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateActor(Actor actor , IFormFile ProfilePictuer)
        {
           var filename = Guid.NewGuid() + Path.GetExtension(ProfilePictuer.FileName);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cast", filename);

            using (var stram = System.IO.File.Create(filePath))
            {
                ProfilePictuer.CopyTo(stram);
            }

            actor.ProfilePictuer = filename;

            _Actor.CreateAsync(actor);
            await _Actor.Commit();


            TempData["Success-Message"] = " Created Successfully Successfully";
            return RedirectToAction("ActorIndex");
        }


        [HttpGet]
        public async Task<IActionResult> EditActor([FromRoute] int id)
        {


            var actor = await _Actor.GetOne(Expression: e => e.Id == id);
                 
            return View(actor);


        }

        [HttpPost]
        public async Task<IActionResult> EditActor(Actor actor, IFormFile ProfilePictuer)
        {

            var act = await _Actor.GetOne(Expression: e => e.Id == actor.Id, asNoTracking: true);


            if (ProfilePictuer is not null && ProfilePictuer.Length > 0)
            {

                var fileName = Guid.NewGuid() + Path.GetExtension(ProfilePictuer.FileName);
                var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cast", fileName);
                using (var stream = System.IO.File.Create(newPath))
                {
                    ProfilePictuer.CopyTo(stream);
                }
                actor.ProfilePictuer = fileName;


                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cast", act.ProfilePictuer);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            else
            {
                actor.ProfilePictuer = act.ProfilePictuer;
            }


            _Actor.Update(actor);
            await _Actor.Commit();


            TempData["Success-Message"] = " Updated Successfully Successfully";

            return RedirectToAction("ActorIndex");
        }


    }
}
