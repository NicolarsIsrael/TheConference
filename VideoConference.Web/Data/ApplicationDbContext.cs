﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Core;

namespace VideoConference.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Meeting> Meeting { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Memo> Memo { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DocumentMinute> DocumentMinute { get; set; }
        public DbSet<Message> Message { get; set; }
    }
}

