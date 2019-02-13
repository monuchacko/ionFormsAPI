using Microsoft.EntityFrameworkCore;

namespace ionForms.API.Entities
{
    public class FDDataContext : DbContext
    {
        public FDDataContext(DbContextOptions<FDDataContext> options)
          : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Column> Columns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Column>()
            .HasIndex(c => new { c.FormId, c.ColumnName }).IsUnique();
        }
    }
}