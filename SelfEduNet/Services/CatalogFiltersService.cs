namespace SelfEduNet.Services
{
	public class CatalogFiltersService
	{
		public string SearchQuery { get; set; }
		public string Language { get; set; }
		public bool? HaveCertificate { get; set; }
		public bool? IsFree { get; set; }
	}
}
