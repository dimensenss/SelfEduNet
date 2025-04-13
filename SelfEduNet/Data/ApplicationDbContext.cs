using SelfEduNet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


using static System.Reflection.Metadata.BlobBuilder;

namespace SelfEduNet.Data
{
	public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		public DbSet<AppUser> Users { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<CourseInfo> CourseInfos { get; set; }
		public DbSet<CourseModules> CourseModules { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<Step> Steps { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<UserStep> UserSteps { get; set; }
		public DbSet<UserLesson> UserLessons { get; set; }
		public DbSet<UserCourse> UserCourses { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Course>()
				.HasOne(c => c.Info)
				.WithOne(ci => ci.Course)
				.HasForeignKey<CourseInfo>(ci => ci.CourseId);

			modelBuilder.Entity<Course>()
				.HasMany(c => c.Modules)
				.WithOne(m => m.Course)
				.HasForeignKey(m => m.CourseId);
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.OwnedCourses)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserCourse>()
                .HasOne(c => c.User)
                .WithMany(u => u.UserCourses)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCourse>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseModules>()
				.HasMany(cm => cm.Lessons)
				.WithOne(l => l.CourseModule)
				.HasForeignKey(l => l.CourseModuleId);

            modelBuilder.Entity<Lesson>()
	            .HasMany(l => l.Steps)
	            .WithOne(s => s.Lesson)
	            .HasForeignKey(s => s.LessonId)
	            .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Category>()
				.HasOne(c => c.Parent)
				.WithMany(c => c.Children)
				.HasForeignKey(c => c.ParentId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<CourseInfo>()
				.HasMany(ci => ci.Authors)
				.WithMany(u => u.AuthoredCourses)
				.UsingEntity<Dictionary<string, object>>(
					"CourseInfoAuthor",
					j => j
						.HasOne<AppUser>()
						.WithMany()
						.HasForeignKey("AuthorId")
						.OnDelete(DeleteBehavior.Cascade),
					j => j
						.HasOne<CourseInfo>()
						.WithMany()
						.HasForeignKey("CourseInfoId") 
						.OnDelete(DeleteBehavior.Cascade)
				);
			modelBuilder.Entity<UserStep>()
				.HasKey(us => new { us.UserId, us.StepId });

			modelBuilder.Entity<UserStep>()
				.HasOne(us => us.User)
				.WithMany(u => u.UserSteps)
				.HasForeignKey(us => us.UserId);

			modelBuilder.Entity<UserStep>()
				.HasOne(us => us.Step)
				.WithMany(s => s.UserSteps)
				.HasForeignKey(us => us.StepId);

			modelBuilder.Entity<UserLesson>()
				.HasKey(us => new { us.UserId, us.LessonId });

			modelBuilder.Entity<UserLesson>()
				.HasOne(us => us.User)
				.WithMany(u => u.UserLessons)
				.HasForeignKey(us => us.UserId);

			modelBuilder.Entity<UserLesson>()
				.HasOne(us => us.Lesson)
				.WithMany(s => s.UserLessons)
				.HasForeignKey(us => us.LessonId);
		}
	}
}



