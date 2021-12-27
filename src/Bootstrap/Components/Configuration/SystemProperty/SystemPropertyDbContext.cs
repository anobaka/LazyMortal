﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Configuration.SystemProperty
{
    public class SystemPropertyDbContext : DbContext
    {
        public DbSet<Models.Entities.SystemProperty> SystemProperties { get; set; }

        public SystemPropertyDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }
    }
}