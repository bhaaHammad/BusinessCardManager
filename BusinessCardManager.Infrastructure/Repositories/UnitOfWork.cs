using BusinessCardManager.Application.Interfaces;
using BusinessCardManager.Infrastructure.Persistence;
using System.Collections;

namespace BusinessCardManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private readonly Hashtable _repositories = new Hashtable();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IBaseRepository<T> Repository<T>() where T : class
        {
            var typeName = typeof(T).Name;

            if (!_repositories.ContainsKey(typeName))
            {
                var repositoryType = typeof(BaseRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(typeName, repositoryInstance);
            }

            return (IBaseRepository<T>)_repositories[typeName];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
