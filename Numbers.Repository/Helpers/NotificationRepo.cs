using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Helpers
{
    public interface INotificationRepository 
    {
        Task<IEnumerable<BellNotification>> GetByNameAsync(string name);
        
        public int GetTotalCS(string to);
    }
    public class NotificationService : INotificationRepository 
    {
        private readonly NumbersDbContext _dbContext;
        public NotificationService(NumbersDbContext context) 
        {
            _dbContext = context;
        }

        public Task<IEnumerable<BellNotification>> GetByNameAsync(string name)
        {
            //return _context.ARCustomers

            return null;
        }
        
        public int  GetTotalCS(string userName)
        {
            int model = 0;
            try
            {
            if (userName== "Hamza")
            {
                 
                    model = _dbContext.APComparativeStatements.FromSql("select distinct  m.* from APComparativeStatements m,APCSRequestDetail d where m.ID =d.APComparativeStatement_ID and m.IsDelete=0  and m.IsApprove=0 and d.GrandTotal >=50000").Count();
                 
            }
            }
            catch (Exception e)
            {

            }
            return model;
           // return null;
        }

        //public IEnumerable<AusFollowUp> GetAll()
        //{
        //    IEnumerable<AusFollowUp> listRepo = _dbContext.AusFollowUps.ToList();
        //    return listRepo;
        //}


        //public string GetTotalFollowUp(string userId)
        //{
        //    var CurrentDate = DateTime.Now.ToShortDateString();
        //    var total =  _dbContext.AusFollowUps.Where(x => x.NextFollowDate ==Convert.ToDateTime(CurrentDate) && x.CreatedBy==userId && x.IsNotified == false).ToList();
        //    string val = Convert.ToString(total.Count());
        //    if(val==null)
        //    {
        //        val = "0";
        //    }
        //    return val;
        //}



    }
}
