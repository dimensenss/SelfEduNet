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
		public DbSet<Category> Categories { get; set; }

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
                .HasOne(c => c.Owner) // Один власник
                .WithMany(u => u.OwnedCourses) // Має багато курсів
                .HasForeignKey(c => c.OwnerId) // Зовнішній ключ
                .OnDelete(DeleteBehavior.Restrict); // Заборонити каскадне видалення
            // Налаштування зв'язку між AppUser і CourseUserRelation
            modelBuilder.Entity<CourseUserRelation>()
                .HasOne(c => c.User) // Один User
                .WithMany(u => u.CourseRelations) // Має багато CourseRelations
                .HasForeignKey(c => c.UserId) // Зовнішній ключ
                .OnDelete(DeleteBehavior.Cascade); // Каскадне видалення (опціонально)

            // Налаштування зв'язку між Course і CourseUserRelation
            modelBuilder.Entity<CourseUserRelation>()
                .HasOne(c => c.Course) // Один Course
                .WithMany() // (опціонально) можна додати UserRelations у модель Course
                .HasForeignKey(c => c.CourseId) // Зовнішній ключ
                .OnDelete(DeleteBehavior.Restrict); // Заборонити каскадне видалення

            modelBuilder.Entity<CourseModules>()
				.HasMany(cm => cm.Lessons)
				.WithOne(l => l.CourseModule)
				.HasForeignKey(l => l.CourseModuleId);

			modelBuilder.Entity<Category>()
				.HasOne(c => c.Parent)
				.WithMany(c => c.Children)
				.HasForeignKey(c => c.ParentId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}



