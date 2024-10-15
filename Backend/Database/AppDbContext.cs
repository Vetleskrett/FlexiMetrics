using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Assignment> Assignments { get; set; }

    public DbSet<AssignmentVariable> AssignmentVariables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Team>()
                    .HasIndex(t => new { t.TeamId, t.CourseId })
                    .IsUnique();
        modelBuilder.Entity<Assignment>()
                    .HasIndex(a => new { a.Name, a.CourseId })
                    .IsUnique();
    }
}