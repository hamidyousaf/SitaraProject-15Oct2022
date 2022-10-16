using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public class FGSInwardGatePass
	{
		public int Id { get; set; }
		[Display(Name = "IGP #")]
		public int IGPNo { get; set; }
		[Display(Name = "IGP Date")]
		public DateTime IGPDate { get; set; }
		[Display(Name = "Warehouse")]
		public int WarehouseId { get; set; }
		[Display(Name = "Vendor")]
		public int VendorId { get; set; }
		[Display(Name = "OGP #")]
		public int OGPId { get; set; }
		[MaxLength(450)]
		[Display(Name = "Driver Name")]
		public string DriverName { get; set; }
		[Display(Name = "Vehicle Type")]
		public int VehicleTypeId { get; set; }
		[MaxLength(50)]
		[Display(Name = "Vehicle #")]
		public string VehicleNo { get; set; }
		[MaxLength(450)]
		public string Remarks { get; set; }
		[MaxLength(450)]
		[Display(Name = "File Attachment")]
		public string FileAttachment { get; set; }
		public bool IsDeleted { get; set; }
		public int Resp_Id { get; set; }
		public int CompanyId { get; set; }
		public bool IsActive { get; set; }
		[MaxLength(450)]
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public bool IsApproved { get; set; }
		[MaxLength(450)]
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		[MaxLength(450)]
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		[MaxLength(15)]
		public string Status { get; set; }
		// navigational property
		public FGSOutwardGatePass OGP {get; set;}
		public AppCompanyConfig VehicleType { get; set;}
		public AppCompanyConfig Warehouse { get; set;}
		public APSupplier Vendor { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}