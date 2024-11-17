using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Назва категорії")]
        public string Title { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "URL")]
        public string Slug { get; set; }

        [Display(Name = "Батьківська категорія")]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Category Parent { get; set; }

        public ICollection<Category> Children { get; set; } = new List<Category>();

        public override string ToString()
        {
            return GetCategoryPath();
        }

        public string GetCategoryPath()
        {
            var path = new List<string>();
            var current = this;
            while (current != null)
            {
                path.Insert(0, current.Title);
                current = current.Parent;
            }
            return string.Join(" > ", path);
        }
    }
}
