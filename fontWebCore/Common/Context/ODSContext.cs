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
        public virtual DbSet<receiveCases> receiveCases { get; set; }
        public virtual DbSet<postfile> postfile { get; set; }
        public virtual DbSet<logJson> logJson { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");
            modelBuilder.Entity<members>().HasKey(o => o.customer_id);
            modelBuilder.Entity<receiveCases>().HasKey(o => o.receive_id);
            modelBuilder.Entity<postfile>().HasKey(o => o.zipcode);
            modelBuilder.Entity<logJson>().HasKey(o => o.log_id);
            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
