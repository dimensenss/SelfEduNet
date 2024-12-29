using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data.Enum;
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
                var coursesInfos = new List<CourseInfo>
                {
                    new CourseInfo
                    {
                        CourseId = 1, // Id курса "Основи програмування на C#"
                        Workload = 40, // Примерная нагрузка
                        PreviewVideo = "https://example.com/csharp_intro_preview", // URL превью-видео
                        Authors = new List<AppUser>
                        {
                            /* авторы курса */
                        }
                    },
                    new CourseInfo
                    {
                        CourseId = 2, // Id курса "Веб-розробка з JavaScript"
                        Workload = 60, // Примерная нагрузка
                        PreviewVideo = "https://example.com/javascript_web_dev_preview", // URL превью-видео
                        Authors = new List<AppUser>
                        {
                            /* авторы курса */
                        }
                    },
                    new CourseInfo
                    {
                        CourseId = 3, // Id курса "Основи Python"
                        Workload = 50, // Примерная нагрузка
                        PreviewVideo = "https://example.com/python_intro_preview", // URL превью-видео
                        Authors = new List<AppUser>
                        {
                            /* авторы курса */
                        }
                    },
                    new CourseInfo
                    {
                        CourseId = 4, // Id курса "Просунутий JavaScript"
                        Workload = 80, // Примерная нагрузка
                        PreviewVideo = "https://example.com/advanced_javascript_preview", // URL превью-видео
                        Authors = new List<AppUser>
                        {
                            /* авторы курса */
                        }
                    },
                    new CourseInfo
                    {
                        CourseId = 5, // Id курса "Промо курс зі створення сайтів"
                        Workload = 20, // Примерная нагрузка
                        PreviewVideo = "https://example.com/promo_course_preview", // URL превью-видео
                        Authors = new List<AppUser>
                        {
                            /* авторы курса */
                        }
                    }
                };
                var courses = new List<Course>
                {
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Основи програмування на C#",
                        Description = "Цей курс допоможе вам вивчити основи програмування на C#.",
                        FullPrice = 0,
                        Language = LanguageType.Ukrainian,
                        Difficulty = DifficultyType.Beginner,
                        HaveCertificate = true,
                        Preview = "/images/csharp_intro.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Info = coursesInfos[0],
                        IsPublished = true,
                        CategoryId = 6 // Курс належить категорії "Програмування"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Веб-розробка з JavaScript",
                        Description = "Дізнайтеся, як створювати динамічні веб-сайти з використанням JavaScript.",
                        FullPrice = 1000,
                        Language = LanguageType.Ukrainian,
                        Difficulty = DifficultyType.Middle,
                        HaveCertificate = false,
                        Preview = "/images/javascript_web_dev.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Info = coursesInfos[1],
                        IsPublished = true,
                        CategoryId = 6 // Курс належить підкатегорії "JavaScript"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Основи Python",
                        Description = "Навчіться основам програмування на Python.",
                        FullPrice = 0,
                        Language = LanguageType.Ukrainian,
                        Difficulty = DifficultyType.Beginner,
                        HaveCertificate = true,
                        Preview = "/images/python_intro.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Info = coursesInfos[2],
                        IsPublished = true,
                        CategoryId = 6 // Курс належить підкатегорії "Python"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Просунутий JavaScript",
                        Description = "Дізнайтеся всі тонкощі JavaScript для розробки складних веб-додатків.",
                        FullPrice = 1500,
                        Language = LanguageType.Ukrainian,
                        Difficulty = DifficultyType.Expert,
                        HaveCertificate = true,
                        Preview = "/images/advanced_javascript.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Info = coursesInfos[3],
                        IsPublished = true,
                        CategoryId = 6 // Курс належить підкатегорії "JavaScript"
                    },
                    new Course
                    {
                        OwnerId = "3d580ca7-b836-408c-8ad6-d95b7cbc4a53",
                        CourseName = "Промо курс зі створення сайтів",
                        Description = "Отримайте безкоштовний доступ до матеріалів по створенню сайтів.",
                        FullPrice = 0,
                        Language = LanguageType.Ukrainian,
                        Difficulty = DifficultyType.Beginner,
                        HaveCertificate = false,
                        Preview = "/images/promo_course.jpg",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Info = coursesInfos[4],
                        IsPublished = true,
                        CategoryId = 7 // Курс належить категорії "Промо"
                    }
                };
                await _context.CourseInfos.AddRangeAsync(coursesInfos);
                await _context.Courses.AddRangeAsync(courses);
                await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних
            }
        }
    }
}
