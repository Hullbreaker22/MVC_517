using Microsoft.AspNetCore.Mvc;

namespace MyCeima.Areas.Admin.Controllers
{

    [Area(SD.AdminArea)]


    public class CinemaController : Controller
    {

            IRepository<Cinema> _Ceima;

        public CinemaController(IRepository<Cinema> cema)
        {
            _Ceima = cema;
        }

        public async Task<IActionResult> CinemaIndex()
        {
            var cima = await _Ceima.GetAllAsync();

            return View(cima);
        }

        [HttpGet]
        public async Task<IActionResult> EditCeima([FromRoute] int id)
        {
            var collect = await _Ceima.GetOne(Expression: e=>e.Id == id);

            return View(collect);
        }

        [HttpPost]
        public async Task<IActionResult> EditCeima(Cinema ceima)
        {
            _Ceima.Update(ceima);
           await _Ceima.Commit();

            TempData["Success-Message"] = " Updated Successfully Successfully";


            return RedirectToAction("CinemaIndex");
        }
        [HttpGet]
        public IActionResult CreateCenima()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCenima(Cinema cinema)
        {
           
            _Ceima.CreateAsync(cinema);
           await _Ceima.Commit();

            TempData["Success-Message"] = " Created Successfully Successfully";

            return RedirectToAction("CinemaIndex");
        }
    }
}
