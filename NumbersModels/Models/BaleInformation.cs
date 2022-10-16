using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class BaleInformation
    {
        private IQueryable<BaleInformation> baleInformations;

       

        public int Id { get; set; }
		public string Brand { get; set; }
		public string Design { get; set; }
		public string BaleType { get; set; }
		public int ProductTypeId { get; set; }
		public int Bales { get; set; }
		public decimal TotalMeters { get; set; }
		public string DesignCode { get; set; }
		public decimal Meters { get; set; }
		public int? NoOfBale { get; set; }
		public int? BaleCode { get; set; }
		public int? ItemId { get; set; }
		public int TempBales { get; set; }
        public bool UsedFNumber { get; set; }
		public string BaleNumber { get; set; }
		public int TransactionNo { get; set; }
		public int ProductionOrderId { get; set; }
		public int ProductionQty { get; set; }
		public int BalProductionQty { get; set; }
		public int TotalFGSQty { get; set; }
		public string LotNo { get; set; }
		public string ProductionOrderNo { get; set; }



		public DateTime TransactionDate { get; set; }
		public int ItemCategory2 { get; set; }
		public int WarehouseId { get; set; }
		public int ItemCategory3 { get; set; }
		public int ItemCategory4 { get; set; }
		[NotMapped]
        public SelectList WareHouseLOV { get; set; }
		//navigational property
		public InvItem Item { get; set; }
	}
}