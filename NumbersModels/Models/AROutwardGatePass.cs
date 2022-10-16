using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public class AROutwardGatePass
	{
		[Key]
		public int Id { get; set; }
		[Display(Name = "OGP #")]
		public int OGPNo { get; set; }
		[Display(Name = "OGP Date")]
		public DateTime OGPDate { get; set; }
		[Display(Name ="Warehouse")]
		public int WarehouseId { get; set; }
		[Display(Name ="Customer")]
		public int CustomerId { get; set; }
		[Display(Name ="SIGP #")]
		public int SIGPId { get; set; }
		[Display(Name = "Bilty #")]
		public string BuiltyNo { get; set; }
		public string Status { get; set; }
		public int Bails { get; set; }
		public decimal OGPQty { get; set; }
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
		// navigational property
		public AppCompanyConfig Warehouse { get; set; }
		public ARCustomer Customer { get; set; }
		public ARSaleReturnInwardGatePass SIGP { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}

}