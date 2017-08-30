using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CmsShoppingCard.Models.Date
{
    public class Db : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
    }
}