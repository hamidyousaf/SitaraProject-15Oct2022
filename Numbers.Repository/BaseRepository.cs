using Numbers.Entity.Models;
using Numbers.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Numbers.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly NumbersDbContext _context;
        internal DbSet<T> dbSet;

        public BaseRepository(NumbersDbContext context)
        {
            _context = context;
            this.dbSet=this._context.Set<T>();
        }
        
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string Include = "")
        {
            IQueryable<T> query = this.dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrEmpty(Include))
            {
                foreach (var item in Include.Split(','))
                {
                    query = query.Include(item);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }
        
        public async Task CreateAsync(T entity)
        {
            _context.Add(entity);
            await SaveAsync();
        }
        public async Task CreateRangeAsync(List<T> entity)
        {
            _context.AddRange(entity);
            await SaveAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await SaveAsync();
        }
        public async Task UpdateRangeAsync(List<T> entity)
        {
            _context.UpdateRange(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await SaveAsync();
        }
        public async Task DeleteRangeAsync(List<T> entity)
        {
            _context.RemoveRange(entity);
            await SaveAsync();
        }

        protected async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

   
    }
}
