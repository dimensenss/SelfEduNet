﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SelfEduNet.Data.Enum;

namespace SelfEduNet.Models
{
    public class Course
    {
        public int Id { get; set; }

        

        [Required]
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public AppUser Owner { get; set; }

        [Required]
        [MaxLength(255)]
        public string CourseName { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Range(0, int.MaxValue)]
        public int FullPrice { get; set; } = 0;

        [Required]
        public LanguageType Language { get; set; }

        public DifficultyType? Difficulty { get; set; }

        public bool HaveCertificate { get; set; } = false;

        public string? Preview { get; set; } // Путь к файлу изображения

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = false;

        public ICollection<CourseModules> Modules { get; set; } = new List<CourseModules>();
        public CourseInfo Info { get; set; }
        public decimal Rating { get; set; }

        public override string ToString()
        {
            return CourseName;
        }
    }
}
