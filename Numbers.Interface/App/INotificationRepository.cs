using Numbers.Entity.Models;
using Numbers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Interface
{
    public interface INotificationRepository 
    {
        Task<IEnumerable<BellNotification>> GetByNameAsync(string name);

    }
}
