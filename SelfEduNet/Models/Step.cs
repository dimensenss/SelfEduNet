﻿using System.ComponentModel.DataAnnotations;
using SelfEduNet.Data.Enum;

namespace SelfEduNet.Models
{
	public class Step
	{
		public int Id { get; set; }
		[Required]
		public int LessonId { get; set; }
		public Lesson? Lesson { get; set; }
		public int Order { get; set; }
		[Required]
		public StepType StepType { get; set; }
		[MaxLength(65000)]
		public string? Content { get; set; }
		[MaxLength(10000)]
		public string? Resume { get; set; }

		[MaxLength(65000)] 
		public string Context { get; set; } = string.Empty;
		[Url]
		public string? VideoUrl { get; set; }
		public bool Required { get; set; } = false;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public ICollection<UserStep> UserSteps { get; set; } = new List<UserStep>();
		public StepTest? StepTest { get; set; }
	}
}
