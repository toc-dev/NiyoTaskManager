using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Data
{
    public class NiyoDbContext: IdentityDbContext<NiyoUser>
    {
        public NiyoDbContext(DbContextOptions<NiyoDbContext> options)
            : base(options)
        {
        }

        public DbSet<NiyoUser> Users { get; set; }
        public DbSet<NiyoTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<NiyoUser>().ToTable("Users").Property(p=>p.Id).HasColumnName("id");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("Login");
            builder.Entity<IdentityRole>().ToTable("Role").Property(p => p.Id).HasColumnName("Id");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");

            builder.Entity<NiyoTask>().HasKey(t => t.Id);
        }
    }




}

