using Microsoft.EntityFrameworkCore;
using EVA_2.Models;


namespace EVA_2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

        {
        }

        public DbSet<Cliente> Cliente { get; set; }


    }

}
