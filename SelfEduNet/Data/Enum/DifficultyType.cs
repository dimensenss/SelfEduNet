using System.ComponentModel.DataAnnotations;

namespace SelfEduNet.Data.Enum
{
	public enum DifficultyType
	{
		[Display(Name = "Починаючий")]
		Beginner,
		[Display(Name = "Середній")]
		Middle,
		[Display(Name = "Продвинутий")]
		Expert
	}
}
