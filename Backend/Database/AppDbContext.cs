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
    public DbSet<CourseTeacher> CourseTeachers { get; set; }
    public DbSet<CourseStudent> CourseStudents { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentField> AssignmentFields { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryField> DeliveryFields { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        var course = modelBuilder.Entity<Course>();
        course.HasMany(c => c.Teams).WithOne(t => t.Course);

        var courseTeacher = modelBuilder.Entity<CourseTeacher>();
        courseTeacher.HasOne(x => x.Course).WithMany();
        courseTeacher.HasOne(x => x.Teacher).WithMany();

        var courseStudent = modelBuilder.Entity<CourseStudent>();
        courseStudent.HasOne(x => x.Course).WithMany();
        courseStudent.HasOne(x => x.Student).WithMany();

        modelBuilder.Entity<Team>().HasMany(t => t.Students).WithMany();

        var assignment = modelBuilder.Entity<Assignment>();
        assignment.HasOne(a => a.Course).WithMany();
        assignment.HasMany(a => a.Fields).WithOne(f => f.Assignment);
        assignment.OwnsOne(a => a.GradingFormat);

        var delivery = modelBuilder.Entity<Delivery>();
        delivery.HasOne(d => d.Assignment).WithMany();
        delivery.HasMany(d => d.Fields).WithOne(f => f.Delivery).OnDelete(DeleteBehavior.Cascade);
        delivery.HasOne(d => d.Student).WithMany().IsRequired(false);
        delivery.HasOne(d => d.Team).WithMany().IsRequired(false);
        delivery.HasIndex(d => new
        {
            d.AssignmentId,
            d.StudentId,
            d.TeamId,
        }).IsUnique();

        modelBuilder.Entity<DeliveryField>()
            .HasOne(f => f.AssignmentField).WithMany();

        var feedback = modelBuilder.Entity<Feedback>();
        feedback.HasOne(f => f.Delivery).WithOne();

        modelBuilder.Entity<ApprovalFeedback>();
        modelBuilder.Entity<LetterFeedback>();
        modelBuilder.Entity<PointsFeedback>();
    }
}