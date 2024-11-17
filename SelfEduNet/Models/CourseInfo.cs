﻿using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models
{
    public class CourseInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Range(0, int.MaxValue)]
        public int Workload { get; set; } = 0;

        public ICollection<AppUser> Authors { get; set; } = new List<AppUser>();

        public string PreviewVideo { get; set; } // URL для превью-видео
    }
}
