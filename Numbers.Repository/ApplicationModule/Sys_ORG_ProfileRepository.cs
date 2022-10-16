using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AppModule
{
    public class Sys_ORG_ProfileRepository : BaseRepository<Sys_ORG_Profile>, ISys_ORG_ProfileRepository
    {
        public Sys_ORG_ProfileRepository(NumbersDbContext context) : base(context)
        {

        }

    }
}
