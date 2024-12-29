using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Data.Enum
{
    public enum StatusType
    {
        [Display(Name = "Всі")]
        All,
        [Display(Name = "Опубліковані")]
        Published,
        [Display(Name = "Чернетки")]
        Draft
    }
}
