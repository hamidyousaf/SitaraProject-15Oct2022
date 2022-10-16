using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using MailKit.Net.Smtp;
//using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
//using System.Net.Mail;

using MailKit.Security;
using MailKit.Net.Smtp;
using Numbers.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Controllers
{
    public interface IBackGroundRefresh
    {
       public  void SaleOrderExpiration();
    }
    public class BackGroundRefresh :  IBackGroundRefresh
    {
        private readonly NumbersDbContext _numbersDbContext;
        public BackGroundRefresh(NumbersDbContext numbersDbContext)
        {
            _numbersDbContext = numbersDbContext;
        }
        [HttpGet]
        public void SaleOrderExpiration1()
        {
            var baleInforModel = _numbersDbContext.BaleInformation.Where(p => p.TempBales == 0).ToList();
            var saleOrder = _numbersDbContext.ARSaleOrderItems.Where(x=>x.Meters!=null).Include(p => p.SaleOrder).Where(p => p.Qty > p.DCQty && p.IsDeleted != true && p.SaleOrder.SaleOrderDate.Date.AddDays(p.SaleOrder.Validity) < DateTime.Now.Date && p.IsExpired != true).ToList();
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
                _numbersDbContext.SaveChanges();
                _numbersDbContext.ARSaleOrderItems.Update(saleOrderItem);
                _numbersDbContext.SaveChanges();
            }
        }

        [HttpGet]
        public void SaleOrderExpiration()
        {
                var baleInforModel = _numbersDbContext.BaleInformation.Where(p => p.TempBales == 0).ToList();
            var saleOrder = _numbersDbContext.ARSaleOrderItems.Where(x => x.Meters != null).Include(p => p.SaleOrder).Where(p => p.Qty > p.DCQty && p.IsDeleted != true && p.SaleOrder.SaleOrderDate.Date.AddDays(p.SaleOrder.Validity) < DateTime.Now.Date && p.IsExpired != true).ToList();
            foreach (var item in saleOrder)
            {
                var baleInfoModelList = new List<BaleInformation>();
                var baleInfoModelListSave = new List<BaleInformation>();
                var baleInfoModel = new BaleInformation();
                var saleOrderItem = new ARSaleOrderItem();

                saleOrderItem = item;
                baleInfoModelList=(baleInforModel.Where(p => p.ItemId == item.ItemId && p.Meters == item.Meters)).ToList();
                if (baleInfoModelList!=null && baleInfoModelList.Count>0)
                {
                    for (int i = 0; i < item.Qty; i++)
                    {
                        baleInfoModel = baleInfoModelList[i];
                        baleInfoModel.TempBales = 1;
                        baleInfoModelListSave.Add(baleInfoModel);
                    }
                    saleOrderItem.IsExpired = true;
                }
                _numbersDbContext.BaleInformation.UpdateRange(baleInfoModelListSave);
                _numbersDbContext.SaveChanges();
                _numbersDbContext.ARSaleOrderItems.Update(saleOrderItem);
                _numbersDbContext.SaveChanges();
            }
        }

    }
}
