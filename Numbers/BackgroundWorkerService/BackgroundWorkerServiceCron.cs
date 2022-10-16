using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.BackgroundWorkerService
{
    public interface IBackgroundWorkerService
    {
        public void SaleOrderExpiration();
    }
    public class BackgroundWorkerServiceCron : IBackgroundWorkerService
    {
        private readonly NumbersDbContext _numbersDbContext;

        public BackgroundWorkerServiceCron(NumbersDbContext numbersDbContext)
        {
            _numbersDbContext = numbersDbContext;
        }
        public void SaleOrderExpiration()
        {
            var baleInforModel = _numbersDbContext.BaleInformation.Where(p => p.TempBales == 0).ToList();
            var saleOrder = _numbersDbContext.ARSaleOrderItems.Include(p => p.SaleOrder).Where(p => p.Qty > p.DCQty && p.IsDeleted != true && p.SaleOrder.SaleOrderDate.Date.AddDays(p.SaleOrder.Validity) < DateTime.Now.Date && p.IsExpired != true).ToList();
            foreach (var item in saleOrder)
            {
                var baleInfoModelList = new List<BaleInformation>();
                var baleInfoModel = new BaleInformation();
                var saleOrderItem = new ARSaleOrderItem();

                saleOrderItem = item;

                if (baleInforModel.Exists(p => p.ItemId == item.ItemId && p.Meters == item.Meters))
                {
                    foreach (var baleinfo in baleInforModel)
                    {
                        baleInfoModel = baleinfo;
                        baleInfoModel.TempBales = 1;
                        baleInfoModelList.Add(baleInfoModel);
                    }
                    saleOrderItem.IsExpired = true;
                }
                _numbersDbContext.BaleInformation.UpdateRange(baleInfoModel);
                _numbersDbContext.ARSaleOrderItems.Update(saleOrderItem);
                _numbersDbContext.SaveChanges();
            }
        }
    }
}   
