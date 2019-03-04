namespace Services.Web
{
	internal class LoadImageDependency
	{
		public readonly string ImageName;

		public readonly string ConfigDataKey;

		public LoadImageDependency(string imageName, string configDataKey = "BrawlDataUrl")
		{
			ImageName = imageName;
			ConfigDataKey = configDataKey;
		}
	}
}
