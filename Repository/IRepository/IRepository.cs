using System.Linq.Expressions;

namespace ETickets.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // CRUD
        void CreateNew(T entity);
        void Edit(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        IEnumerable<T> Get(Expression<Func<T, bool>> expression,
                                      params Expression<Func<T, object>>[] includeProperties);
        void Commit();
    }
}
