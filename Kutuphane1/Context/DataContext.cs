using Kutuphane1.Models;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane1.Context
{
	public class DataContext : DbContext
	{
		//public DataContext() { }

		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{ 
		}
		public DataContext() : base(GetOptions())
		{

		}

		private static DbContextOptions GetOptions()
		{
			string conn_str = "Server=SAIMGUL\\SQLEXPRESS;Database=Kutuphane12;Trusted_Connection=True;";
			return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), conn_str).Options;
		}


		public DbSet<OrtakRaf> OrtakRaf { get; set; }
		public DbSet<KitapAdi> KitapAdi { get; set; }
		public DbSet<SayisalKitap> SayisalKitap { get; set; }
	}
}