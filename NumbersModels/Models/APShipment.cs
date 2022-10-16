using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public class APShipment
    {
		[Key]
        public int Id { get; set; }
        public int ShipmentNo { get; set; }
        public DateTime ShipmentDate { get; set; }
		public int BOLNo { get; set; }
		public DateTime BOLDate { get; set; }
		public string BOLType { get; set; }
		public int LCNo { get; set; }
		public string DischargePort { get; set; }
		public string Vendor { get; set; }
		public string ContainerNo { get; set; }
		[Column(TypeName = "numeric(18,4)")]
		public decimal Weight { get; set; }
		public string CBM { get; set; }
		public string Terminal { get; set; }
		public string ShipmentAgent { get; set; }
		public string Currency { get; set; }
		public string TransporterName { get; set; }
		public int BuiltyNo { get; set; }
		public DateTime BuiltyDate { get; set; }
		public string Acceptance { get; set; }
		public DateTime DocRealDate { get; set; }
		public DateTime MaturityDate { get; set; }
		[Column(TypeName = "numeric(18,4)")]
		public decimal ExchangeRate { get; set; }
		public string Attachment { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int Resp_Id { get; set; }
	}
}