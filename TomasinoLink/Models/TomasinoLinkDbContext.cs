using Microsoft.EntityFrameworkCore;
using TomasinoLink.Models;

public class TomasinoLinkDbContext : DbContext
{
    public TomasinoLinkDbContext(DbContextOptions<TomasinoLinkDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } // Now you need to explicitly declare this

    // Other DbSets for your application
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Linked> Linked { get; set; }

    // ... any other DbSet declarations

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.UserId)
            .ValueGeneratedOnAdd(); // This ensures the ID is auto-generated

        base.OnModelCreating(modelBuilder);

        // Configure User-Linked relationship
        modelBuilder.Entity<Linked>()
            .HasOne(l => l.LinkInitiator)
            .WithMany(u => u.InitiatedLinks)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Configure the delete behavior as needed

        modelBuilder.Entity<Linked>()
            .HasOne(l => l.LinkedUser)
            .WithMany(u => u.ReceivedLinks)
            .HasForeignKey(l => l.LinkId)
            .OnDelete(DeleteBehavior.Restrict); // Configure the delete behavior as needed
    }
}