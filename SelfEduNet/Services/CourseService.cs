using CloudinaryDotNet;
using EduProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfEduNet.Data;
using SelfEduNet.Data.Enum;
using SelfEduNet.Models;
using SelfEduNet.Repositories;
using SelfEduNet.ViewModels;

namespace SelfEduNet.Services
{
    public interface ICourseService
	{
		Task<EditCourseViewModel> GetEditViewModelAsync(int id);
		Task<bool> UpdateCourseAsync(int id, EditCourseViewModel courseVM);
		Task<bool> UpdateModuleAsync(CourseModules module);
		Task<bool> UpdateLessonAsync(Lesson lesson);
		Task<bool> CreateCourseAsync(CreateCourseViewModel courseVM);
		Task<bool> CreateModuleAsync(int courseId);
		Task<bool> CreateLessonAsync(int moduleId, string title);
		Task<Lesson> GetLessonByIdAsync(int? lessonId, string userId = null);
		Task<Lesson?> GetNextLessonAsync(int courseId, int lessonId);
		Task<LessonStatistics> GetLessonStatisticsByIdAsync(int lessonId, string userId);
		Task<CourseContentViewModel> GetCourseContentViewModelAsync(int id);
		Task<IEnumerable<Course>> GetAllCoursesAsync();
		IQueryable<Course> GetAllCoursesByOwnerQuery(string userId);
		Task<IEnumerable<Course>> GetCoursesByOwnerAsync(string userId);
		Task<CourseModules> GetModuleByIdAsync(int moduleId);
		Task<bool> UpdateModuleOrderAsync(List<ModuleOrderViewModel> moduleOrder);
		Task<IEnumerable<CourseModules>> GetAllModulesByCourseId(int courseId);
		Task<Course> GetCourseByIdAsync(int id);
		Task<Course> GetCourseWithInfoByIdAsync(int id);
		public Task<int> GetMaxLessonOrderAsync(int moduleId);
		public Task<int> GetMaxModuleOrderAsync(int courseId);
		public bool DeleteCourse(Course course);
		public bool DeleteModule(CourseModules module);
		public bool DeleteLesson(Lesson lesson);
		public bool Update(Course course);
	}
	public class CourseService(ApplicationDbContext context, ICourseRepository courseRepository, ICategoryService categoryService, IStepRepository stepRepository,
		IPhotoService photoService): ICourseService
	{
		private readonly ApplicationDbContext _context = context;
		private readonly ICourseRepository _courseRepository = courseRepository;
		private readonly ICategoryService _categoryService = categoryService;
		private readonly IStepRepository _stepRepository = stepRepository;
		private readonly IPhotoService _photoService = photoService;

		public async Task<EditCourseViewModel> GetEditViewModelAsync(int id)
		{
			var course = await _courseRepository.GetCourseWithInfoByIdAsync(id);
			if (course == null) return null;

			var courseVM = new EditCourseViewModel
			{
				Id = course.Id,
				PreviewURL = course.Preview,
				CourseName = course.CourseName,
				Description = course.Description,
				PromoText = course.PromoText,
				Language = course.Language,
				Difficulty = course.Difficulty,
				FullPrice = course.FullPrice,
				HaveCertificate = course.HaveCertificate,
				Workload = course.Info.Workload,
				Authors = course.Info.Authors,
				Modules = course.Modules,
				PreviewVideo = course.Info.PreviewVideo,
				IsPublished = course.IsPublished,
				Category = course.Category,
			};
			
			return courseVM;
		}

		public async Task<Lesson?> GetNextLessonAsync(int courseId, int lessonId)
		{
			return await _courseRepository.GetNextLessonAsync(courseId, lessonId);
		}

		public async Task<LessonStatistics> GetLessonStatisticsByIdAsync(int lessonId, string userId)
		{
			var steps = await _stepRepository.GetStepsByLessonIdAsync(lessonId, userId);
			return _courseRepository.GetLessonStatisticsByIdAsync(steps);
		}

		public async Task<CourseContentViewModel> GetCourseContentViewModelAsync(int id)
		{
			var course = await _courseRepository.GetCourseWithModulesByIdAsync(id);
			if (course == null) return null;

			var courseVM = new CourseContentViewModel
			{
				Id = course.Id,
				CourseName = course.CourseName,
				Modules = course.Modules,
				Lessons = course.Modules
					.SelectMany(module => module.Lessons ?? new List<Lesson>()) // Flatten lessons from all modules
					.ToList()
			};

			return courseVM;
		}

		public IQueryable<Course> GetAllCoursesByOwnerQuery(string userId)
		{
			return _courseRepository.GetAllCoursesByOwnerQuery(userId);
		}

		public async Task<bool> UpdateCourseAsync(int id, EditCourseViewModel courseVM)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			{
				try
				{
					var course = await _courseRepository.GetCourseWithInfoByIdAsync(id);
					if (course == null) return false;

					string previewURL = courseVM.PreviewURL;
					if (courseVM.Preview != null)
					{
						try
						{
							var fileInfo = new FileInfo(course.Preview);
							string publicId = Path.GetFileNameWithoutExtension(fileInfo.Name);
							await _photoService.DeleteFileAsync(publicId);
						}
						catch
						{
							//TODO : log error
						}

						var resultPhoto = await _photoService.AddPhotoAsync(courseVM.Preview);
						previewURL = resultPhoto.Url.ToString();
					}

					course.Info.Workload = courseVM.Workload;
					course.Info.PreviewVideo = courseVM.PreviewVideo;

					course.Preview = previewURL;
					course.CourseName = courseVM.CourseName;
					course.Description = courseVM.Description;
					course.Language = courseVM.Language;
					course.Difficulty = courseVM.Difficulty;
					course.FullPrice = courseVM.FullPrice;
					course.HaveCertificate = courseVM.HaveCertificate;
					course.IsPublished = courseVM.IsPublished;

					bool result = _courseRepository.Update(course);
					await transaction.CommitAsync();

					return result;
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					return false;
				}
			}
		}

		public async Task<bool> CreateCourseAsync(CreateCourseViewModel courseVM)
		{
			var course = new Course
			{
				OwnerId = courseVM.OwnerId,
				Owner = courseVM.Owner,
				CourseName = courseVM.CourseName
			};

			var draftsCategory = await _categoryService.GetOrCreateDraftsCategoryAsync();
			course.Category = draftsCategory;

			return await courseRepository.AddAsync(course);
		}

		public async Task<bool> UpdateModuleAsync(CourseModules module)
		{
			return await courseRepository.UpdateModuleAsync(module);
		}
		public async Task<bool> UpdateLessonAsync(Lesson lesson)
		{
			return await courseRepository.UpdateLessonAsync(lesson);
		}

		private async Task<string> GenerateNewModuleTitle(int courseId)
		{
			var existingModules = await _courseRepository.GetAllModulesByCourseId(courseId);
			var existingTitles = existingModules.Select(m => m.Title);

			int moduleNumber = 1;
			while (existingTitles.Contains($"Новий модуль {moduleNumber}"))
			{
				moduleNumber++;
			}

			return $"Новий модуль {moduleNumber}";
		}

		public async Task<bool> CreateModuleAsync(int courseId)
		{
			string title = await GenerateNewModuleTitle(courseId);
			int maxOrder = await GetMaxModuleOrderAsync(courseId);
			var module = new CourseModules
            {
				Title = title,
                CourseId = courseId,
				Order = maxOrder + 1
            };
            return await _courseRepository.AddModuleAsync(module);
        }
		public async Task<int> GetMaxLessonOrderAsync(int moduleId)
		{
			return await _courseRepository.GetMaxLessonOrderAsync(moduleId);
		}
		public async Task<int> GetMaxModuleOrderAsync(int courseId)
		{
			return await _courseRepository.GetMaxModuleOrderAsync(courseId);
		}
		public async Task<bool> CreateLessonAsync(int moduleId, string title)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			{
				try
				{
					int maxOrder = await _courseRepository.GetMaxLessonOrderAsync(moduleId);
					var module = await _courseRepository.GetModuleByIdAsync(moduleId);
					var lesson = new Lesson
					{
						CourseModuleId = moduleId,
						CourseId = module.CourseId,
						Title = title,
						Order = maxOrder + 1
					};

					bool lessonSaved = await _courseRepository.AddLessonAsync(lesson);
					if (!lessonSaved)
					{
						await transaction.RollbackAsync();
						return false;
					}

					var firstStep = new Step
					{
						LessonId = lesson.Id,
						Content = "Урок створений системою",
						StepType = StepType.Text,
						Order = 1
					};

					bool stepSaved = await _stepRepository.AddAsync(firstStep);
					if (!stepSaved)
					{
						await transaction.RollbackAsync();
						return false;
					}

					await transaction.CommitAsync();
					return true;
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					return false;
				}
			}
		}

		public async Task<Lesson> GetLessonByIdAsync(int? lessonId, string userId = null)
		{
			return await _courseRepository.GetLessonByIdAsync(lessonId, userId);
		}

		public async Task<CourseModules> GetModuleByIdAsync(int moduleId)
        {
            return await _courseRepository.GetModuleByIdAsync(moduleId);
        }

        public async Task<IEnumerable<CourseModules>> GetAllModulesByCourseId(int courseId)
        {
	        return await _courseRepository.GetAllModulesByCourseId(courseId);
		}
        public async Task<bool> UpdateModuleOrderAsync(List<ModuleOrderViewModel> moduleOrder)
        {
	        foreach (var module in moduleOrder)
	        {
		        var existingModule = await _courseRepository.GetModuleByIdAsync(module.ModuleId);
		        if (existingModule != null)
		        {
			        existingModule.Order = module.Order;
			        await _courseRepository.UpdateModuleAsync(existingModule);
		        }
		        else
		        {
			        return false;
		        }
	        }

	        return true;
        }

		public async Task<IEnumerable<Course>> GetAllCoursesAsync()
		{
			return await _courseRepository.GetAllCoursesAsync();
		}

		public async Task<IEnumerable<Course>> GetCoursesByOwnerAsync(string userId)
		{
			return await _courseRepository.GetCoursesByOwnerAsync(userId);
		}
        

        public async Task<Course> GetCourseByIdAsync(int id)
		{
			return await _courseRepository.GetCourseByIdAsync(id);
		}

		public async Task<Course> GetCourseWithInfoByIdAsync(int id)
		{
			return await _courseRepository.GetCourseWithInfoByIdAsync(id);
		}

		public bool DeleteCourse(Course course)
		{
			return _courseRepository.Delete(course);
		}
		public bool DeleteModule(CourseModules module)
		{
			return _courseRepository.DeleteModule(module);
		}
		public bool DeleteLesson(Lesson lesson)
		{
			return _courseRepository.DeleteLesson(lesson);
		}

		public bool Update(Course course)
		{
			return _courseRepository.Update(course);
		}
	}
}
