using Microsoft.AspNetCore.Mvc;
using SelfEduNet.Data.Enum;
using SelfEduNet.Extensions;
using SelfEduNet.Models;
using static SelfEduNet.Models.Course;

namespace SelfEduNet.ViewModels
{
    public class CourseFilter
    {
        public string? Query { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        [ModelBinder(BinderType = typeof(CheckboxBinder))]
        public bool? HaveCertificate { get; set; }
        [ModelBinder(BinderType = typeof(CheckboxBinder))]
        public bool? IsFree { get; set; }
        public string? Language { get; set; }
        public string? Difficulty { get; set; }
        public string? Owner { get; set; }
        public string? Status { get; set; }
    }
}
