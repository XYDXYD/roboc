using Avatars;
using ExtensionMethods;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace SocialServiceLayer
{
	internal class MultiAvatarLoadRequest : IMultiAvatarLoadRequest, IServiceRequest<MultiAvatarRequestDependency>, IAnswerOnComplete<Texture2D>, ITask, IServiceRequest, IAbstractTask
	{
		private static float AVATAR_LOAD_TIMEOUT_SECONDS = 5f;

		private IServiceAnswer<Texture2D> _answer;

		private string _avatarName;

		private AvatarType _avatarType;

		public bool isDone
		{
			get;
			private set;
		}

		public void Inject(MultiAvatarRequestDependency dependency)
		{
			_avatarName = dependency.name;
			_avatarType = dependency.avatarType;
		}

		public void Execute()
		{
			Texture2D avatar = CacheDTO.AvatarCache.GetAvatar(_avatarType, _avatarName);
			if (avatar != null)
			{
				Complete(avatar);
			}
			else
			{
				TaskRunner.get_Instance().Run(LoadAvatarAsync(_avatarType, _avatarName));
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer<Texture2D> answer)
		{
			_answer = answer;
			return this;
		}

		private IEnumerator LoadAvatarAsync(AvatarType avatarType, string avatarName)
		{
			if (!((avatarType != 0) ? ClientConfigData.TryGetValue("ClanAvatarCdnUrl", out string cdnurl) : ClientConfigData.TryGetValue("AvatarCdnUrl", out cdnurl)))
			{
				throw new Exception("Unable to load CDN URL");
			}
			string fullurl = (avatarType != AvatarType.ClanAvatar) ? (cdnurl + avatarName) : (cdnurl + avatarName.ToLower());
			if (!cdnurl.StartsWith("http") && !cdnurl.Contains("://"))
			{
				fullurl = "http://" + fullurl;
			}
			WWW www = new WWW(fullurl);
			try
			{
				yield return (object)new WWWEnumerator(www, AVATAR_LOAD_TIMEOUT_SECONDS);
				if (www.get_isDone() && www.get_error().IsNullOrWhiteSpace())
				{
					byte[] bytes = www.get_bytes();
					Texture2D texture = AvatarUtils.DeserialiseAvatar(new CustomAvatarInfo(bytes));
					CacheDTO.AvatarCache.SetAvatar(avatarType, avatarName, texture);
					Complete(texture);
				}
				else
				{
					CacheDTO.AvatarCache.SetAvatar(avatarType, avatarName, AvatarUtils.FailedToLoadTexture);
					Dictionary<string, string> data;
					if (www.get_isDone())
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("url", www.get_url());
						dictionary.Add("response", www.get_responseHeaders().ToPrettyString());
						data = dictionary;
						int responseCode = www.GetResponseCode();
						if (responseCode == 404)
						{
							Failed("No avatar found at " + fullurl);
						}
						else
						{
							Failed("Error loading avatar at " + fullurl + " : " + www.get_error());
						}
					}
					else
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("url", www.get_url());
						dictionary.Add("response", "custom time out");
						data = dictionary;
						Failed("Error loading avatar at " + fullurl + " : timed out waiting for response");
					}
					RemoteLogger.Error("Error loading avatar", www.get_error(), null, data);
				}
			}
			finally
			{
				base._003C_003E__Finally0();
			}
		}

		private void Failed(string error)
		{
			if (_answer != null && _answer.failed != null)
			{
				_answer.failed(new ServiceBehaviour(Localization.Get("strRobocloudError", true), string.Format("{0} : {1}", Localization.Get("strErrorGettingAvatarInfo", true), error)));
			}
			else
			{
				Console.LogError("avatar WWW failed: " + error);
			}
			isDone = true;
		}

		private void Complete(Texture2D texture)
		{
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(texture);
			}
			else
			{
				Console.LogWarning("No answer to invoke after loading avatar");
			}
			isDone = true;
		}
	}
}
