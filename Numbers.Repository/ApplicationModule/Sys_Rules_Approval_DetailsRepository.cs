using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Numbers.Entity.Models;
using Numbers.Interface;
using Numbers.Entity.ViewModels;

namespace Numbers.Repository.AppModule
{
    public class Sys_Rules_Approval_DetailsRepository : BaseRepository<Sys_Rules_Approval_Details>, ISys_Rules_Approval_DetailsRepository
    {
        public Sys_Rules_Approval_DetailsRepository(NumbersDbContext context) : base(context)
        {

        }


    }
}
