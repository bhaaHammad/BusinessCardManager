using BusinessCardManager.Application.Interfaces;

namespace BusinessCardManager.Tests.TestDoubles
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repos = new();
        public int SaveChangesCalls { get; private set; } = 0;

        public IBaseRepository<T> Repository<T>() where T : class
        {
            var t = typeof(T);
            if (!_repos.TryGetValue(t, out var repo))
            {
                repo = new InMemoryRepository<T>();
                _repos[t] = repo;
            }
            return (IBaseRepository<T>)repo;
        }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }

        public void Dispose()
        {
        }

        public InMemoryRepository<T> GetInMemoryRepo<T>() where T : class
            => (InMemoryRepository<T>)Repository<T>();
    }
}