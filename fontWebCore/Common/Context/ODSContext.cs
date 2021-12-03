using fontWebCore.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace fontWebCore.Common.Context
{
    public class ODSContext : DbContext
    {
        public ODSContext(DbContextOptions<ODSContext> options) : base(options)
        {

        }
        public virtual DbSet<members> members { get; set; }
        public virtual DbSet<recevieCases> recevieCases { get; set; }
        public virtual DbSet<postfile> postfile { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");
            modelBuilder.Entity<members>().HasKey(o => o.customer_id);
            modelBuilder.Entity<recevieCases>().HasKey(o => o.recevie_id);
            modelBuilder.Entity<postfile>().HasKey(o => o.zipcode);
            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
