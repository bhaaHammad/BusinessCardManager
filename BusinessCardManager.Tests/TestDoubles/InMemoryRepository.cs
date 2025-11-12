using System.Linq.Expressions;
using BusinessCardManager.Application.Interfaces;

namespace BusinessCardManager.Tests.TestDoubles
{
    public class InMemoryRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly List<T> _items = new();

        public Task<T> GetByIdAsync(int id)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null) return Task.FromResult<T>(null);
            foreach (var i in _items)
            {
                var v = prop.GetValue(i);
                if (v is int iv && iv == id) return Task.FromResult(i);
            }
            return Task.FromResult<T>(null);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_items.ToList());
        }

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            var compiled = predicate.Compile();
            return Task.FromResult<IEnumerable<T>>(_items.Where(compiled).ToList());
        }

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
        {
            var compiled = predicate?.Compile();
            var q = _items.AsEnumerable();
            if (compiled != null) q = q.Where(compiled);
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            q = q.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return Task.FromResult<IEnumerable<T>>(q.ToList());
        }

        public Task AddAsync(T entity)
        {
            _items.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(T entity)
        {
            // No-op for this fake (reference types updated by caller)
        }

        public void Delete(T entity)
        {
            _items.Remove(entity);
        }

        // Test-only helpers
        public IReadOnlyList<T> Items => _items;
        public void Clear() => _items.Clear();
    }
}