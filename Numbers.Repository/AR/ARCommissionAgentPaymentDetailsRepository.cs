using Numbers.Entity.Models;
using Numbers.Interface.AR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
    public class ARCommissionAgentPaymentGenerationDetailsRepository : BaseRepository<ARCommissionAgentPaymentGenerationDetails>, IARCommissionAgentPaymentDetailsRepository
    {
        public ARCommissionAgentPaymentGenerationDetailsRepository(NumbersDbContext context) : base(context)
        {

        }
    }
}