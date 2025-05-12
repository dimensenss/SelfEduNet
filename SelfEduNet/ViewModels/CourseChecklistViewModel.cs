using SelfEduNet.Models;

namespace SelfEduNet.ViewModels;
public class CourseChecklistViewModel
{
	public Course Course { get; set; }
	public int TotalModules { get; set; }
	public int TotalLessons { get; set; }
	public int TotalSteps { get; set; }
	public int EmptyModules { get; set; }
	public int DefaultModuleTitles { get; set; }
	public int DefaultTexts { get; set; }
	public int MissingVideos { get; set; }
	public int MissingTests { get; set; }

	public bool PreviewIsLoaded { get; set; }
	public bool CategoryIsAttached { get; set; }
	public bool PromoTextExists { get; set; }

	public int StructureCompleted =>
		(TotalModules >= 2 ? 1 : 0) +
		(TotalLessons >= 10 ? 1 : 0) +
		(TotalSteps >= 10 ? 1 : 0) +
		(EmptyModules == 0 ? 1 : 0) +
		(DefaultModuleTitles == 0 ? 1 : 0) +
		(DefaultTexts == 0 ? 1 : 0) +
		(MissingVideos == 0 ? 1 : 0) +
		(MissingTests == 0 ? 1 : 0);

	public int StructureTotal => 8;

	public int PresentationCompleted =>
		(PreviewIsLoaded ? 1 : 0) +
		(CategoryIsAttached ? 1 : 0) +
		(PromoTextExists ? 1 : 0);

	public int PresentationTotal => 3;
}

