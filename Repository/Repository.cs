using Microsoft.EntityFrameworkCore;
using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using System.Linq.Expressions;

namespace ETickets.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public void Commit() => context.SaveChanges();
        public void CreateNew(T entity) => dbSet.Add(entity);
        public void Delete(T entity) => dbSet.Remove(entity);
        public void Edit(T entity) => dbSet.Update(entity);
        public IEnumerable<T> GetAll() => dbSet.ToList();
        public IEnumerable<T> Get(Expression<Func<T, bool>> expression,
                                      params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;
            
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return query.Where(expression);
        }

    }
}
