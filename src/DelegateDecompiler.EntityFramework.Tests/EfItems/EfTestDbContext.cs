// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes;
#if EF_CORE
using System.Configuration;
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DbModelBuilder = Microsoft.EntityFrameworkCore.ModelBuilder;
#else
using System.Data.Entity;
#endif

namespace DelegateDecompiler.EntityFramework.Tests.EfItems
{
    public class EfTestDbContext : DbContext
    {
#if EF_CORE
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var location = new Uri(typeof(EfTestDbContext).Assembly.EscapedCodeBase).LocalPath;
            var configuration = ConfigurationManager.OpenExeConfiguration(location);
            var connectionString = configuration.ConnectionStrings.ConnectionStrings["DelegateDecompilerEfTestDb"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }
#else
        static EfTestDbContext() 
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<EfTestDbContext>()); 
        }

        public EfTestDbContext() : base("name=DelegateDecompilerEfTestDb") { }
#endif

        public DbSet<EfParent> EfParents { get; set; }
        public DbSet<EfChild> EfChildren { get; set; }
        public DbSet<EfGrandChild> EfGrandChildren { get; set; }

        public DbSet<EfPerson> EfPersons { get; set; }

        public DbSet<LivingBeeing> LivingBeeing { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EfPerson>()
                .Computed(x => x.FullNameNoAttibute)
                .Computed(x => x.GetFullNameNoAttibute());
            
            modelBuilder.Entity<Dog>();
            modelBuilder.Entity<HoneyBee>();
            modelBuilder.Entity<Person>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
