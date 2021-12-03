// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Configuration;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes;
#if EF_CORE
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            var connectionString = GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
#if EF_CORE5
            optionsBuilder.LogTo((id, _) => id == RelationalEventId.CommandExecuted, d =>
            {
                if (Log != null && d is CommandExecutedEventData e)
                {
                    Log(e.Command.CommandText);
                }
            });
#endif
        }
#else
        static EfTestDbContext() 
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<EfTestDbContext>()); 
        }

#if NETFRAMEWORK
        public EfTestDbContext() : base("name=DelegateDecompilerEfTestDb") { }
#else
        public EfTestDbContext() : base(GetConnectionString()) { }
#endif
#endif

#if NETCOREAPP
        static string GetConnectionString()
        {
            var location = new Uri(typeof(EfTestDbContext).Assembly.EscapedCodeBase).LocalPath;
            var configuration = ConfigurationManager.OpenExeConfiguration(location);
            var connectionString = configuration.ConnectionStrings.ConnectionStrings["DelegateDecompilerEfTestDb"]
                .ConnectionString;
            return connectionString;
        }
#endif

        public DbSet<EfParent> EfParents { get; set; }
        public DbSet<EfChild> EfChildren { get; set; }
        public DbSet<EfGrandChild> EfGrandChildren { get; set; }

        public DbSet<EfPerson> EfPersons { get; set; }

        public DbSet<LivingBeeing> LivingBeeing { get; set; }

        public Action<string> Log { get; set; }

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
