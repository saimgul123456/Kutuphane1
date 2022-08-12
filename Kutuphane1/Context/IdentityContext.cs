using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kutuphane1.ModelsIdentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane1.Context
{
	public class IdentityContext : IdentityDbContext<CustomUser, CustomRole, string>
  {
    //StartUp.cs üzerinden ayarlanır.
    public IdentityContext(DbContextOptions<IdentityContext> opt) : base(opt)
    {

    }

   
    public IdentityContext() : base(GetOptions())
    {

    }

    private static DbContextOptions GetOptions()
    {
      string conn_str = "Server=SAIMGUL\\SQLEXPRESS;Database=Kutuphane12Identity;Trusted_Connection=True;";
      
      return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), conn_str).Options;
    }
  }
}
