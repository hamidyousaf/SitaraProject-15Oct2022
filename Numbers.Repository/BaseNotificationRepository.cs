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
    public interface IBaseNotificationRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
 
    }
    public class BaseNotificationRepository<T> : IBaseNotificationRepository<T> where T : class
    {
        protected readonly NumbersDbContext _context;
        internal DbSet<T> dbSet;

        public BaseNotificationRepository(NumbersDbContext context)
        {
            _context = context;
            this.dbSet=this._context.Set<T>();
        }
        
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

 
   
    }
}
