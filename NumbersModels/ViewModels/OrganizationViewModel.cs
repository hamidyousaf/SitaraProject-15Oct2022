using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
  public class OrganizationViewModel
    {
        //Master
        public int Organization_Id { get; set; }
        public string OrgName { get; set; }
        public string OrgType { get; set; }
        public DateTime EffectiveFromDate { get; set; }
        public DateTime EffectiveToDate { get; set; }
        public string OrgLocation { get; set; }
        public string OrgInternalExternal { get; set; }
        public string OrgAddress { get; set; }
        public string OrgIntAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string ShortName { get; set; }

        public int CompanyId { get; set; }

        //Detail
        public int ClassificationDetailId { get; set; }
        public int OrganizationId { get; set; }
        public int DocId { get; set; }
        public int ClassificationId { get; set; }
        public bool IsEnable { get; set; }
        public int Ledger_Id { get; set; }
        public int OperationUnitId { get; set; }
        public int BusinessUnitId { get; set; }

        //navigation Property
        public List<SYS_FORMS> Forms { get; set; }
        public List<SysOrgClassification> InvStoreIssueItems { get; set; }


    }
}
