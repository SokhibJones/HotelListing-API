using HotelListing.Data;
using System.Linq.Expressions;
using X.PagedList;

namespace HotelListing.Core.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<IPagedList<T>> GetAllAsync(RequestParams requestParam);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(int id);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);


    }
}
