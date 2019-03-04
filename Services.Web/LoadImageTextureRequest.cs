using ExtensionMethods;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Services.Web
{
	internal class LoadImageTextureRequest : ILoadImageTextureRequest, IServiceRequest<LoadImageDependency>, IAnswerOnComplete<Texture2D>, IServiceRequest
	{
		private const string ERR_FETCH_IMG = "Error fetching brawl/campaign image.";

		private const string ERR_IMG_NOT_FOUND = "The brawl/campaign image was not found.'";

		private IServiceAnswer<Texture2D> _answer;

		private LoadImageDependency _dependency;

		private static readonly Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

		public bool isDone
		{
			get;
			private set;
		}

		public void Inject(LoadImageDependency dependency)
		{
			_dependency = dependency;
		}

		public void Execute()
		{
			if (_cache.TryGetValue(_dependency.ImageName, out Texture2D value))
			{
				HandleLoadImageRequestComplete(value);
			}
			else
			{
				TaskRunner.get_Instance().Run(LoadRequest(_dependency.ImageName));
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer<Texture2D> answer)
		{
			_answer = answer;
			return this;
		}

		private IEnumerator LoadRequest(string imageName)
		{
			if (ClientConfigData.TryGetValue(_dependency.ConfigDataKey, out string cdnurl))
			{
				string imageFullURL = cdnurl + imageName;
				WWW www = new WWW(imageFullURL);
				try
				{
					yield return (object)new WWWEnumerator(www, 5f);
					string error = www.get_error();
					if (www.get_isDone() && error.IsNullOrWhiteSpace())
					{
						byte[] bytes = www.get_bytes();
						Texture2D val = UnpackImage(bytes);
						_cache[_dependency.ImageName] = val;
						HandleLoadImageRequestComplete(val);
					}
					else
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("imageUrl", imageFullURL);
						Dictionary<string, string> dictionary2 = dictionary;
						if (www.get_isDone())
						{
							int responseCode = www.GetResponseCode();
							if (responseCode == 404)
							{
								HandleLoadImageRequestFailed("The brawl/campaign image was not found.'", dictionary2);
							}
							else
							{
								dictionary2.Add("wwwError", error);
								HandleLoadImageRequestFailed("Error fetching brawl/campaign image.", dictionary2);
							}
						}
						else
						{
							dictionary2.Add("wwwError", error);
							dictionary2.Add("reasonImgNotFetched", "custom time out");
							HandleLoadImageRequestFailed("Error fetching brawl/campaign image.", dictionary2);
						}
					}
				}
				finally
				{
					base._003C_003E__Finally0();
				}
			}
			else
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("reasonImgNotFetched", "unable to load CDN URL!");
				dictionary.Add("cdnUrlKey", _dependency.ConfigDataKey);
				Dictionary<string, string> data = dictionary;
				HandleLoadImageRequestFailed("Error fetching brawl/campaign image.", data);
			}
		}

		private static Texture2D UnpackImage(byte[] bytes)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			Texture2D val = new Texture2D(100, 100, 10, false);
			try
			{
				ImageConversion.LoadImage(val, bytes);
				return val;
			}
			catch (Exception arg)
			{
				Console.LogError("Failed to deserialise brawl image. Exception thrown: " + arg);
				return null;
			}
		}

		private void HandleLoadImageRequestFailed(string error, Dictionary<string, string> data)
		{
			RemoteLogger.Error(error, string.Empty, string.Empty, data);
			Texture2D failedToLoadTexture = AvatarUtils.FailedToLoadTexture;
			HandleLoadImageRequestComplete(failedToLoadTexture);
		}

		private void HandleLoadImageRequestComplete(Texture2D texture)
		{
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(texture);
			}
			else
			{
				Console.LogWarning("No answer to invoke after loading brawl image");
			}
			isDone = true;
		}
	}
}
