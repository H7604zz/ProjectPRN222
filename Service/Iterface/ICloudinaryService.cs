namespace ProjectPrn222.Service.Iterface
{
	public interface ICloudinaryService
	{
		Task<string> UploadImageAsync(IFormFile file);
	}
}
