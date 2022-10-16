using Numbers.Entity.Models;
using Numbers.Interface.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AR
{
    public class ARCommissionAgentPaymentGenerationRepository : BaseRepository<ARCommissionAgentPaymentGeneration>, IARCommissionAgentPaymentRepository
    {
        public ARCommissionAgentPaymentGenerationRepository(NumbersDbContext context) : base(context)
        {

        }

    }
}