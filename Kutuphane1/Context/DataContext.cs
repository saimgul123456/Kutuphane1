using Kutuphane1.Models;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane1.Context
{
    public class DataContext : DbContext
    {
        public DataContext() { }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<OrtakRaf> OrtakRaf { get; set; }
        public DbSet<KitapAdi> KitapAdi { get; set; }
        public DbSet<SayisalKitap> SayisalKitap { get; set; }
    }
}