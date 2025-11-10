using System.Linq.Expressions;

namespace BusinessCardManager.Application.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, int pageNumber, int pageSize);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
