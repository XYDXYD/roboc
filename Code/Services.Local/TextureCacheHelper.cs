using Authentication;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Services.Local
{
	internal static class TextureCacheHelper
	{
		private static string _cacheFolder;

		public static string GetGarageThumbnailVersionFilePath()
		{
			HashAlgorithm hashAlgorithm = new SHA1CryptoServiceProvider();
			string username = User.Username;
			byte[] bytes = Encoding.UTF8.GetBytes(username);
			byte[] array = hashAlgorithm.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				stringBuilder.Append(array[i]);
			}
			return string.Format("{0}/{1}", GetCacheFolder(), "ThumbnailVersions_" + stringBuilder + "_garage.txt");
		}

		public static string GetCacheFolder()
		{
			if (_cacheFolder == null)
			{
				_cacheFolder = Application.get_dataPath() + "/../RobotShopThumbnailsCache";
			}
			return _cacheFolder;
		}

		public static string GetCachePath(string url)
		{
			int num = url.LastIndexOf('/');
			if (num == -1)
			{
				return GetCacheFolder() + url;
			}
			return GetCacheFolder() + url.Substring(num);
		}
	}
}
