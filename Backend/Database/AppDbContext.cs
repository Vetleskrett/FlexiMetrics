using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentField> AssignmentFields { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        var course = modelBuilder.Entity<Course>();
        course.HasMany(c => c.Teachers).WithMany();
        course.HasMany(c => c.Students).WithMany();
        course.HasMany(c => c.Teams).WithOne(t => t.Course);

        modelBuilder.Entity<Team>().HasMany(t => t.Students).WithMany();

        var assignment = modelBuilder.Entity<Assignment>();
        assignment.HasOne(a => a.Course);
        assignment.HasMany(a => a.Fields).WithOne(f => f.Assignment);
    }
}