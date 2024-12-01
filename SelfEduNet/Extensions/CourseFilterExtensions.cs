using Microsoft.IdentityModel.Tokens;
using SelfEduNet.Models;
using SelfEduNet.ViewModels;
using static SelfEduNet.Models.Course;

namespace SelfEduNet.Extensions
{
    public static class CourseFilterExtensions
    {
        public static IQueryable<Course> ApplyFilters(this IQueryable<Course> query, CourseFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Query))
            {
                query = query.Where(course =>
                    course.CourseName.Contains(filter.Query) ||
                    course.Description.Contains(filter.Query));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(course => course.FullPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(course => course.FullPrice <= filter.MaxPrice.Value);
            }

            if (filter.HaveCertificate.HasValue)
            {
	            if (filter.HaveCertificate.Value)
	            {
		            query = query.Where(course => course.HaveCertificate == filter.HaveCertificate.Value);
				}
            }

            if (filter.IsFree.HasValue && filter.IsFree.Value)
            {
	            if (filter.IsFree.Value)
	            {
		            query = query.Where(course => course.FullPrice == 0);
	            }
            }

            if (!string.IsNullOrEmpty(filter.Language) && Enum.TryParse<LanguageType>(filter.Language, true, out var language))
            {
                query = query.Where(course => course.Language == language);
            }

            if (!string.IsNullOrEmpty(filter.Difficulty) && Enum.TryParse<DifficultyType>(filter.Difficulty, true, out var difficulty))
            {
                query = query.Where(course => course.Difficulty == difficulty);
            }

            if (!string.IsNullOrEmpty(filter.Owner))
            {
                query = query.Where(course => course.OwnerId.Contains(filter.Owner));
            }

            if (filter.Status.HasValue)
            {
                if (filter.Status.Value == 1)
                {
                    query = query.Where(course => course.IsPublished);
                }
                else if (filter.Status.Value == 2)
                {
                    query = query.Where(course => !course.IsPublished);
                }
            }

            return query;
        }
    }

}
