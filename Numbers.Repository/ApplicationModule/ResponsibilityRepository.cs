using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AppModule
{
    public class ResponsibilityRepository : BaseRepository<Responsibilities>, IResponsibiltyRepository
    {
        public ResponsibilityRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}
