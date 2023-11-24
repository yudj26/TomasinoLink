using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TomasinoLink.Models;

public class TomasinoLinkDbContext : DbContext
{
    public TomasinoLinkDbContext(DbContextOptions<TomasinoLinkDbContext> options) : base(options)
    {
    }

    public DbSet<Photo> Photos { get; set; }
    // ... DbSet properties for other entities

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Optional: Customize your model conventions here
    }
}
