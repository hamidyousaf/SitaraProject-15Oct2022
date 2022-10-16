using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AR
{
    public class CustomerRepository : BaseRepository<ARCustomer>, IARCustomerRepository
    {
        public CustomerRepository(NumbersDbContext context) : base(context)
        {

        }

        public Task<IEnumerable<ARCustomer>> GetByNameAsync(string name)
        {
            //return _context.ARCustomers
            
            return null;
        }
    }
}
