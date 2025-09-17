using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCeima.Models;
using MyCeima.ViewModel;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace MyCeima.Areas.Customer.Controllers
{
    [Area(SD.Customer)]
    [Authorize]
    public class CartController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        public IRepository<MovieCart> _Carts;
        public IRepository<Movies> _Movie;
        public IRepository<ActorMovie> _ActorMovies;
        public IRepository<Promotions> _Promotions;
        public CartController(UserManager<ApplicationUser> user, IRepository<MovieCart> cart, IRepository<Movies> movie, IRepository<ActorMovie> actorMovies, IRepository<Promotions> pro)
        {
            _userManager = user;
            _Carts = cart;
            _Movie = movie;
            _ActorMovies = actorMovies;
            _Promotions = pro;
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]

        public async Task<IActionResult> AddToCart([FromRoute] int id)
        {


            var mov = await _Movie.GetOne(Includes: [e => e.Category, e => e.Cinema, e => e.ActorMovie], Expression: e => e.Id == id);


            return View(mov);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CartRequest teckets)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();


            var currentUser = await _Carts.GetOne(Expression: e => e.ApplicationUserId == user.Id && e.MoviesId == teckets.MoviesId);

            if (currentUser is null)
            {
                await _Carts.CreateAsync(new MovieCart()
                {
                    ApplicationUserId = user.Id,
                    MoviesId = teckets.MoviesId,
                    Count = teckets.Count
                });
            }
            else
            {
                currentUser.Count += teckets.Count;
            }




            await _Carts.Commit();

            TempData["Success-Message"] = "Booked Successfully";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        public async Task<IActionResult> Products(string promoCode)
        {

            var AllCarts = await _Carts.GetAllAsync(Includes: [e => e.Movies]);
            var totalPrice = AllCarts.Sum(x => x.Movies.Price * x.Count);

            if (promoCode is not null)
            {
                var promo = await _Promotions.GetOne(Expression: e => e.Code == promoCode);
                if (promo is null || promo.Usage >= 5)
                {
                    TempData["Error-Message"] = "Invalid Promo";
                }
                else
                {
                    promo.Usage += 1;
                    totalPrice = totalPrice - (totalPrice * 0.05);
                    TempData["Success-Message"] = "Done Successfully!";
                    await _Promotions.Commit();
                }
            }
            ViewBag.TotalPrice = totalPrice;

            return View(AllCarts);
        }

        [HttpPost]
        public async Task<IActionResult> DecrementCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);

            var Cart = await _Carts.GetOne(Expression: e => e.MoviesId == productId && e.ApplicationUserId == user.Id);

            Cart.Count -= 1;

            await _Carts.Commit();

            return RedirectToAction("Products");
        }


        [HttpPost]
        public async Task<IActionResult> IncrementCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var Cart = await _Carts.GetOne(Expression: e => e.MoviesId == productId && e.ApplicationUserId == user.Id);

            Cart.Count += 1;

            await _Carts.Commit();

            return RedirectToAction("Products");
        }


        public async Task<IActionResult> DeleteCart(int id)
        {

            var user = await _userManager.GetUserAsync(User);
            var SameCart = await _Carts.GetOne(Expression: e => e.MoviesId == id && e.ApplicationUserId == user.Id);
            if (SameCart is null)
                return BadRequest();

            _Carts.Delete(SameCart);
            await _Carts.Commit();
            TempData["Success-Message"] = "Deleted Successfully";


            return RedirectToAction("Products");
        }

         public async Task<IActionResult> Pay()
          {

              var user = await _userManager.GetUserAsync(User);

            var cats = await _Carts.GetAllAsync(Expression: e => e.ApplicationUserId == user.Id, Includes: [e => e.Movies]);



              var options = new SessionCreateOptions          
              {
                 PaymentMethodTypes = new List<string> { "card" },
                 LineItems = new List<SessionLineItemOptions>(),
                 Mode = "payment",
                 SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success",
                 CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/cancel",
              };


            foreach (var item in cats)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movies.Name,
                            Description = item.Movies.Description
                        },
                        UnitAmount = (long)(item.Movies.Price * 100), 
                    },
                    Quantity = item.Count,
                });
            }



            var service = new SessionService();
              var session = service.Create(options);


              return Redirect(session.Url);

          }

    }
}
