using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class InwardGatePassViewModel
    {
		public int Id { get; set; }
		[Display(Name = "IGP #")]
		public int IGPNo { get; set; }
		[Display(Name = "IGP Date")]
		public DateTime IGPDate { get; set; }
		[Display(Name = "Warehouse")]
		public int WarehouseId { get; set; }
		[Display(Name = "Customer")]
		public int CustomerId { get; set; }
		[Display(Name = "OGP #")]
		public int OGPId { get; set; }
		[Display(Name = "Bilty #")]
		public string BuiltyNo { get; set; }
		public int BaleBalance { get; set; }
		[Display(Name = "Bales")]
		public decimal Bails { get; set; }

		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int Resp_ID { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string UnApprovedBy { get; set; }
		public string Address { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public SelectList CustomerLOV { get; set; }
		public ARInwardGatePass ARInwardGatePass { get; set; }
		public string Date { get; set; }
	}
}
