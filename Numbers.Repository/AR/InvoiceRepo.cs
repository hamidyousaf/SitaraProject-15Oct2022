using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.AR
{
    public class InvoiceRepo
    {
        private readonly NumbersDbContext _dbContext;
        public InvoiceRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
