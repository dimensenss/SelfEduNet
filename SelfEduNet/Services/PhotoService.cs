using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using EduProject.Helpers;
using System.Text.RegularExpressions;

namespace EduProject.Services
{

	public interface IPhotoService
	{
		Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
		Task<VideoUploadResult> AddVideoAsync(IFormFile file);

		Task<DeletionResult> DeleteFileAsync(string publicId);
		List<string> ExtractImageUrls(string htmlContent);

	}
	public class PhotoService : IPhotoService
	{
		private readonly Cloudinary _cloudinary;

		public PhotoService(IOptions<CloudinarySettings> config)
		{
			var acc = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret
			);
			_cloudinary = new Cloudinary(acc);
		}
		public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
		{
			var uploadResult = new ImageUploadResult();
			if (file.Length > 0)
			{
				await using(var stream = file.OpenReadStream())
				{
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(file.FileName, stream),
						Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
					};
					uploadResult = await _cloudinary.UploadAsync(uploadParams);
				}
			}
			return uploadResult;
		}

		public async Task<VideoUploadResult> AddVideoAsync(IFormFile file)
		{
			var uploadResult = new VideoUploadResult();
			if (file.Length > 0)
			{
				await using (var stream = file.OpenReadStream())
				{
					var uploadParams = new VideoUploadParams()
					{
						File = new FileDescription(file.FileName, stream),
						//Transformation = new Transformation()
						//	.Width(1280).Height(720).Crop("limit")
						//	.BitRate("800k")
						//	.Quality("auto:good")
					};
					uploadResult = await _cloudinary.UploadAsync(uploadParams);
				}
			}
			return uploadResult;
		}

		public async Task<DeletionResult> DeleteFileAsync(string publicId)
		{
			var deleteParams = new DeletionParams(publicId);
			var result = await _cloudinary.DestroyAsync(deleteParams);
			return result;
		}
		public List<string> ExtractImageUrls(string htmlContent)
		{
			var imageUrls = new List<string>();

			var htmlRegex = new Regex("<img[^>]+src=[\"']([^\"']+)[\"']", RegexOptions.IgnoreCase);
			var htmlMatches = htmlRegex.Matches(htmlContent);

			var markdownRegex = new Regex(@"!\[\]\((.*?)\)", RegexOptions.IgnoreCase);
			var markdownMatches = markdownRegex.Matches(htmlContent);

			foreach (Match match in htmlMatches)
			{
				imageUrls.Add(match.Groups[1].Value);
			}

			foreach (Match match in markdownMatches)
			{
				imageUrls.Add(match.Groups[1].Value);
			}

			return imageUrls;
		}
	}
}
