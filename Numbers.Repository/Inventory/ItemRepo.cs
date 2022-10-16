using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Inventory
{
    public class ItemRepo
    {
        private readonly NumbersDbContext _dbContext;
        public ItemRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //for photo uploading function
        public async Task<string> UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\item-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await  img.CopyToAsync(Fstream);
                        var fullPath = "/uploads/item-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }
        
        public IEnumerable<InvItem> GetAll(int companyId)
        {
            IEnumerable<InvItem> listRepo= _dbContext.InvItems.Where(v => v.IsDeleted == false && v.CompanyId == companyId)
                .OrderByDescending(v => v.Id).ToList();
            return listRepo;
        }

        public InvItem GetById(int id)
        {
            var item= _dbContext.InvItems.Find(id);
            return item;
        }
        //public string GetBarCode(int id)
        //{
        //    var result = _dbContext.InvItems.Where(x => x.IsActive && x.CompanyId == id).ToList();
        //    if (result.Count > 0)
        //    {
        //        double Code = _dbContext.InvItems.Select(x => x.Barcode).Max() + 1;
        //        string Barcode = Convert.ToDouble(Code).ToString("00000000000");
        //        return Barcode;
        //    }
        //    else
        //    {
        //        string Barcode = "00000000001";
        //        return Barcode;
        //    }
        //}
        [HttpPost]
        public async Task<bool> Create(InvItem model, IFormFile Photo)
        {
            try
            { 
                var item = new InvItem();
                item.Barcode = model.Barcode;
                item.Code = model.Code;
                item.Name = model.Name;
                item.SalesRate = model.SalesRate;
                item.PurchaseRate = model.PurchaseRate;
                item.Unit = model.Unit;
                item.ManufacturedId = model.ManufacturedId;
                item.InvItemAccountId = model.InvItemAccountId;
                item.StockAccountId = model.StockAccountId;
                item.CustomerStockAccountId = model.CustomerStockAccountId;
                item.SaleAccountId = model.SaleAccountId;
                item.CostofSaleAccountId = model.CostofSaleAccountId;
                item.CategoryId = model.CategoryId;
                item.ItemType = model.ItemType;
                item.GLConsumptionAccountId = model.GLConsumptionAccountId;
                item.DiscountPercentage = model.DiscountPercentage;
                item.ManufacturedCode = model.ManufacturedCode;
                item.SerialNo = model.SerialNo;
                item.ModelNo = model.ModelNo;
                item.Description = model.Description;
                item.Length = model.Length;
                item.Width = model.Width;
                item.Radius = model.Radius;
                item.UOMHeight = model.UOMHeight;
                item.UOMLength = model.UOMLength;
                item.UOMRadius = model.UOMRadius;
                item.UOMSize = model.UOMSize;
                item.UOMWeight = model.UOMWeight;
                item.UOMWidth = model.UOMWidth;
                item.Season = model.Season;
                item.FabricConstruction = model.FabricConstruction;
                item.Brand = model.Brand;
                item.IsSaleable = model.IsSaleable;
                item.IsPurchaseable = model.IsPurchaseable;
                item.Height = model.Height;
                item.IsExpired = model.IsExpired;
                item.IsLabTest = model.IsLabTest;
                item.IsLocation = model.IsLocation;
                item.LotNo = model.LotNo;
                item.Make = model.Make;
                item.Material = model.Material;
                item.MaxStockLevel = model.MaxStockLevel;
                item.MinStockLevel = model.MinStockLevel;
                item.ReorderStockLevel = model.ReorderStockLevel;
                item.WareHouse = model.WareHouse;
                item.Weight = model.Weight;
                item.PackQty = model.PackQty;
                item.PackUnit = model.PackUnit;
                item.OrderType = model.OrderType;
                item.Size = model.Size;
                item.Material = model.Material;
                item.Color = model.Color;
                item.LocationIfTrue = model.LocationIfTrue;
                item.Model = model.Model;
                if (Photo != null)
                {
                    item.Photo =await UploadFile(Photo);
                }
                item.IsActive =true;
                item.CompanyId = model.CompanyId;
                item.IsDeleted = false;
                item.CreatedBy = model.CreatedBy;
                item.CreatedDate = DateTime.Now;                                                                    
                _dbContext.InvItems.Add(item);
                await _dbContext.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(InvItem model, IFormFile Photo)
        {
            var obj = _dbContext.InvItems.Find(model.Id);
            obj.Barcode = model.Barcode;
            obj.Code = model.Code;
            obj.Name = model.Name;
            obj.SalesRate = model.SalesRate;
            obj.PurchaseRate = model.PurchaseRate;
            obj.Unit = model.Unit;
            obj.ManufacturedId = model.ManufacturedId;
            obj.InvItemAccountId = model.InvItemAccountId;
            obj.StockAccountId = model.StockAccountId;
            obj.CustomerStockAccountId = model.CustomerStockAccountId;
            obj.SaleAccountId = model.SaleAccountId;
            obj.CostofSaleAccountId = model.CostofSaleAccountId;
            obj.CategoryId = model.CategoryId;
            obj.ItemType = model.ItemType;
            obj.GLConsumptionAccountId = model.GLConsumptionAccountId;
            obj.DiscountPercentage = model.DiscountPercentage;
            obj.ManufacturedCode = model.ManufacturedCode;
            obj.SerialNo = model.SerialNo;
            obj.ModelNo = model.ModelNo;
            obj.Description = model.Description;
            obj.Length = model.Length;
            obj.Width = model.Width;
            obj.Radius = model.Radius;
            obj.Brand = model.Brand;
            obj.IsSaleable = model.IsSaleable;
            obj.IsPurchaseable = model.IsPurchaseable;
            obj.Height = model.Height;
            obj.IsExpired = model.IsExpired;
            obj.IsLabTest = model.IsLabTest;
            obj.IsLocation = model.IsLocation;
            obj.LotNo = model.LotNo;
            obj.Make = model.Make;
            obj.Material = model.Material;
            obj.MaxStockLevel = model.MaxStockLevel;
            obj.MinStockLevel = model.MinStockLevel;
            obj.ReorderStockLevel = model.ReorderStockLevel;
            obj.WareHouse = model.WareHouse;
            obj.Weight = model.Weight;
            obj.PackQty = model.PackQty;
            obj.PackUnit = model.PackUnit;
            obj.OrderType = model.OrderType;
            obj.Size = model.Size;
            obj.Material = model.Material;
            obj.Color = model.Color;
            obj.LocationIfTrue = model.LocationIfTrue;
            obj.Model = model.Model;
            obj.UOMHeight = model.UOMHeight;
            obj.UOMLength = model.UOMLength;
            obj.UOMRadius = model.UOMRadius;
            obj.UOMSize = model.UOMSize;
            obj.UOMWeight = model.UOMWeight;
            obj.UOMWidth = model.UOMWidth;
            obj.Season = model.Season;
            obj.FabricConstruction = model.FabricConstruction;
            if (Photo != null)
            {
                obj.Photo =await UploadFile(Photo);
            }
            else 
            {
                _dbContext.Entry(obj).State = EntityState.Modified;
                _dbContext.Entry(obj).Property(x => x.Photo).IsModified = false;
            }
            obj.IsActive = true;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            var entry = _dbContext.InvItems.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var deleteItem = _dbContext.InvItems.Where(v => v.IsDeleted == false && v.IsActive == true && v.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.InvItems.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;  
            }
        }

        //public string GetLastItemCode(int companyId)
        //{
        //    var item = _dbContext.InvItems.Where(i => i.IsDeleted == false && i.CompanyId == companyId).OrderByDescending(i => i.Id).FirstOrDefault();
        //    if(item == null)
        //    {
        //        var code= "0";
        //        return code;
        //    }
        //    else
        //    {
        //        return item.Code;
        //    }
        //}

        //private bool IsDuplicateCode(string code, int id)
        //{
        //    var items = _dbContext.InvItems.Any(c => c.Code == code && c.Id != id && c.IsDeleted == false && c.CompanyId == _companyId);
        //    if (items == true)
        //        return true;
        //    else
        //        return false;
        //}


    }
}
