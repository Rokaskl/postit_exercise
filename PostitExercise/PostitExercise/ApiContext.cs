using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace PostitExercise
{
    public class ApiContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable(tb => tb.HasTrigger("ClientsAudit_tr"));
        }
    }
}