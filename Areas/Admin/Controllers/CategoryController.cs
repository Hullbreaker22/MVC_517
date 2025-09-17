using Microsoft.AspNetCore.Mvc;
using MyCeima.ViewModel;

namespace MyCeima.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class CategoryController : Controller
    {

        IRepository<Category> _Category;

        public CategoryController(IRepository<Category> category)
        {
            _Category = category;
        }
        public async Task<IActionResult> CategoriesIndex()
        {

            var Category = await _Category.GetAllAsync();         
            return View(Category);

        }
        [HttpGet]
        public IActionResult CreateCategory()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {

            await _Category.CreateAsync(category);
            await _Category.Commit();


            TempData["Success-Message"] = " Created Successfully Successfully";

            return RedirectToAction("CategoriesIndex");
        }
        [HttpGet]
        public async Task<IActionResult> EditCategory([FromRoute] int id)
        {
            var collect = await _Category.GetOne(Expression: e => e.Id == id);

            return View(collect);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            _Category.Update(category);
            await _Category.Commit();


            TempData["Success-Message"] = " Edit Successfully Successfully";

            return RedirectToAction("CategoriesIndex");
        }
    }
}
