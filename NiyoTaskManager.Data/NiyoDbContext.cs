using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity properties and relationships here
        }
    }




}

