using Microsoft.EntityFrameworkCore;
using SelfEduNet.Models;

namespace SelfEduNet.Data
{
    public class Seeder
    {
        private readonly ApplicationDbContext _context;

        public Seeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {

            if (!_context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Основи програмування на C#",
                        Description = "Цей курс допоможе вам вивчити основи програмування на C#.",
                        FullPrice = 0,
                        Language = Course.LanguageType.Ukrainian,
                        Difficulty = Course.DifficultyType.Beginner,
                        HaveCertificate = true,
                        Preview = "/images/csharp_intro.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsPublished = true,
                        CategoryId = 5 // Курс належить категорії "Програмування"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Веб-розробка з JavaScript",
                        Description = "Дізнайтеся, як створювати динамічні веб-сайти з використанням JavaScript.",
                        FullPrice = 1000,
                        Language = Course.LanguageType.Ukrainian,
                        Difficulty = Course.DifficultyType.Middle,
                        HaveCertificate = false,
                        Preview = "/images/javascript_web_dev.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsPublished = true,
                        CategoryId = 7 // Курс належить підкатегорії "JavaScript"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Основи Python",
                        Description = "Навчіться основам програмування на Python.",
                        FullPrice = 0,
                        Language = Course.LanguageType.Ukrainian,
                        Difficulty = Course.DifficultyType.Beginner,
                        HaveCertificate = true,
                        Preview = "/images/python_intro.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsPublished = true,
                        CategoryId = 7 // Курс належить підкатегорії "Python"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Просунутий JavaScript",
                        Description = "Дізнайтеся всі тонкощі JavaScript для розробки складних веб-додатків.",
                        FullPrice = 1500,
                        Language = Course.LanguageType.Ukrainian,
                        Difficulty = Course.DifficultyType.Expert,
                        HaveCertificate = true,
                        Preview = "/images/advanced_javascript.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsPublished = true,
                        CategoryId = 6 // Курс належить підкатегорії "JavaScript"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Промо курс зі створення сайтів",
                        Description = "Отримайте безкоштовний доступ до матеріалів по створенню сайтів.",
                        FullPrice = 0,
                        Language = Course.LanguageType.Ukrainian,
                        Difficulty = Course.DifficultyType.Beginner,
                        HaveCertificate = false,
                        Preview = "/images/promo_course.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsPublished = true,
                        CategoryId = 6 // Курс належить категорії "Промо"
                    }
                };

                await _context.Courses.AddRangeAsync(courses);
                await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних
            }
        }
    }
}
