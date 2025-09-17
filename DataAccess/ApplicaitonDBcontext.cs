using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyCeima.ViewModel;
using MyCeima.Models;

namespace MyCeima.DataAccess
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Actor> Actors { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movies> Movies { get; set; }
        public DbSet<UserOTP> UserOTPs { get; set; }
        public DbSet<MovieCart> MovieCarts { get; set; }
        public DbSet<FinalCart> FinalCarts { get; set; }
        public DbSet<Promotions> Promotions { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options) 
        {



        }


    }
}
