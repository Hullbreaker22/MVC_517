using Microsoft.EntityFrameworkCore;
using MyCeima.DataAccess;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyCeima.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        ApplicationDBContext _context;
        DbSet<T> _dbSet;

        public Repository(ApplicationDBContext context)
        {

            _context = context;

            _dbSet =  _context.Set<T>();
           
        }

        public async Task  CreateAsync(T enitity)
        {
          await _dbSet.AddAsync(enitity);

        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);

        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);

        }


        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }



        public async Task<List<T>> GetAllAsync(Expression<Func<T,bool>>? Expression = null, Expression<Func<T, object>>[]? Includes = null, bool asNoTracking = false)
        {
            var entites = _dbSet.AsQueryable();
            
            if(Expression is not null)
            {
                entites = entites.Where(Expression);
            }

            if(Includes is not null)
            {
                foreach(var item in Includes)
                {
                    entites = entites.Include(item);
                }
            }

            if (asNoTracking)
            {
                entites = entites.AsNoTracking();
            }


            return await entites.ToListAsync();
        }

        public async Task<T?> GetOne(Expression<Func<T, bool>>? Expression , Expression<Func<T, object>>[]? Includes = null, bool asNoTracking = false)
        {
            return (await GetAllAsync(Expression, Includes, asNoTracking)).FirstOrDefault();
        }




    }
}
