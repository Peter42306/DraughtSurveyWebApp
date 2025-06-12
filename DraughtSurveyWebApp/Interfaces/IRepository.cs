using System.Linq.Expressions;

namespace DraughtSurveyWebApp.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        //Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        //Task<IEnumerable<T>> GetSortedAsync(Expression<Func<T, object>> orderBy);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
    }
}
