﻿using HotelListing.Core.IRepository;
using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using X.PagedList;

namespace HotelListing.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }
        public async Task Delete(int id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity is not null)
            {
                dbSet.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public async Task<IPagedList<T>> GetAllAsync(RequestParams requestParam)
        {
            return await dbSet.ToPagedListAsync(pageNumber: requestParam.PageNumber, pageSize: requestParam.PageSize);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task Insert(T entity)
        {
            await dbSet.AddAsync(entity);

        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
