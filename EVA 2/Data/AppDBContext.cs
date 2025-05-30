using Microsoft.EntityFrameworkCore;
using EVA_2.Models;


namespace EVA_2.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)

        {
        }

        public DbSet<Cliente> Cliente { get; set; }


    }

}
