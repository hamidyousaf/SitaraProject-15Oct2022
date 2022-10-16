using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class PackingViewModel
    {
		public int Id { get; set; }
		[Display(Name = "Trans. #")]
		public int PackingNo { get; set; }
		[Display(Name = "Trans. Date")]
		public DateTime PackingDate { get; set; }

		[Display(Name = "Customer")]
		public int CustomerId { get; set; }
		[Display(Name = "SRI #")]
		public int SRIId { get; set; }
		public int WareHouseId { get; set; }

		public string Address { get; set; }
		public string Status { get; set; }

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
		public DateTime UnApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public SelectList CustomerLOV { get; set; }
		public SelectList ReasonType { get; set; }
		public SelectList ReturnType { get; set; }
		public SelectList Season { get; set; }
		public ARPacking ARPacking { get; set; }
 
		public ARPackingItems PackingItems { get; set; }
		public ARPackingItems[] PackingItemsList { get; set; }
		public string Date { get; set; }
	}
}
