using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MyCeima.Repository.IRepository
{
    public interface IRepository<T> where T : class 
    {

        Task CreateAsync(T enitity);
        void Update(T entity);
        void Delete(T entity);
        Task Commit();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? Expression = null, Expression<Func<T, object>>[]? Includes = null, bool asNoTracking = false);

        Task<T?> GetOne(Expression<Func<T, bool>>? Expression , Expression<Func<T, object>>[]? Includes = null, bool asNoTracking = false);
      
    }
}
