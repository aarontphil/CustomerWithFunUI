using Microsoft.EntityFrameworkCore;
using JustCRM.API.Entities;

namespace JustCRM.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<CustomerEmployment> CustomerEmployments { get; set; }
        public DbSet<CustomerDocument> CustomerDocuments { get; set; }
        public DbSet<Industry> Industries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Table mappings
            modelBuilder.Entity<Customer>().ToTable("CUSTOMERS");
            modelBuilder.Entity<CustomerContact>().ToTable("CUSTOMER_CONTACTS");
            modelBuilder.Entity<CustomerEmployment>().ToTable("CUSTOMER_EMPLOYMENT");
            modelBuilder.Entity<CustomerDocument>().ToTable("CUSTOMER_DOCUMENTS");
            modelBuilder.Entity<Industry>().ToTable("INDUSTRIES");

            // 2. Primary Keys
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<CustomerContact>().HasKey(cc => cc.ContactId);
            modelBuilder.Entity<CustomerEmployment>().HasKey(ce => ce.EmploymentId);
            modelBuilder.Entity<CustomerDocument>().HasKey(cd => cd.DocumentId);
            modelBuilder.Entity<Industry>().HasKey(i => i.IndustryId);

            // 3. Relationships
            // Customer <-> CustomerContact (One-to-One)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CustomerContact)
                .WithOne(cc => cc.Customer)
                .HasForeignKey<CustomerContact>(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer <-> CustomerEmployment (One-to-One)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CustomerEmployment)
                .WithOne(ce => ce.Customer)
                .HasForeignKey<CustomerEmployment>(ce => ce.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer -> CustomerDocuments (One-to-Many)
            modelBuilder.Entity<CustomerDocument>()
                .HasOne(cd => cd.Customer)
                .WithMany(c => c.CustomerDocuments)
                .HasForeignKey(cd => cd.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // CustomerEmployment -> Industry (Many-to-One)
            modelBuilder.Entity<CustomerEmployment>()
                .HasOne(ce => ce.Industry)
                .WithMany(i => i.CustomerEmployments)
                .HasForeignKey(ce => ce.IndustryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Soft Delete Query Filters
            modelBuilder.Entity<Customer>().HasQueryFilter(c => c.IsActive);
            modelBuilder.Entity<CustomerContact>().HasQueryFilter(cc => cc.IsActive);
            modelBuilder.Entity<CustomerEmployment>().HasQueryFilter(ce => ce.IsActive);
            modelBuilder.Entity<CustomerDocument>().HasQueryFilter(cd => cd.IsActive);
            modelBuilder.Entity<Industry>().HasQueryFilter(i => i.IsActive);

            // 5. Defaults
            modelBuilder.Entity<Customer>()
                .Property(c => c.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Customer>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // 6. Seed Data for Industries
            modelBuilder.Entity<Industry>().HasData(
                new Industry { IndustryId = 1, IndustryName = "Technology", IsActive = true },
                new Industry { IndustryId = 2, IndustryName = "Healthcare", IsActive = true },
                new Industry { IndustryId = 3, IndustryName = "Finance", IsActive = true },
                new Industry { IndustryId = 4, IndustryName = "Education", IsActive = true },
                new Industry { IndustryId = 5, IndustryName = "Manufacturing", IsActive = true }
            );
        }
    }
}

