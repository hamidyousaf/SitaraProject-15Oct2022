using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class GRConstruction
	{
        private IQueryable<GRConstruction> GRConstructions;

        public int Id { get; set; }
		public int ItemCategoryId { get; set; }
		public int TransactionNo { get; set; }
		public DateTime TransactionDate { get; set; }
		public int Reed { get; set; }
		public int Pick { get; set; }
		[ForeignKey("Warp")]
		public int WarpId { get; set; }
		[ForeignKey("Weft")]
		public int WeftId { get; set; }
		[ForeignKey("GRCategory")]
		public int GRCategoryId { get; set; }
		public int Threads { get; set; }
		public string  Description { get; set; }
		public int CompanyId { get; set; }
		public bool IsDeleted { get; set; }

		[MaxLength(450)]
		public string CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
		public string ApprovedBy { get; set; }	
		public DateTime ApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public string Status { get; set; }

		public InvItem Weft { get; set; }
		public InvItem Warp { get; set; }
        [NotMapped]
		public GRConstruction construction { get; set; }
		public GRCategory GRCategory { get; set; }
		[NotMapped]
		public SelectList GRCategoryLOV { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}