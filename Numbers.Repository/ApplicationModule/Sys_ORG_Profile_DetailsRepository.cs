using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;

namespace Numbers.Repository.AppModule
{
    public class Sys_ORG_Profile_DetailsRepository : BaseRepository<Sys_ORG_Profile_Details>, ISys_ORG_Profile_DetailsRepository
    {
        public Sys_ORG_Profile_DetailsRepository(NumbersDbContext context) : base(context)
        {

        }

    }
}
