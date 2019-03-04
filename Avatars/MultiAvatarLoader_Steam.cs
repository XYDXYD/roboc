using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Avatars
{
	internal class MultiAvatarLoader_Steam : IMultiAvatarLoader
	{
		[Inject]
		internal ISocialRequestFactory SocialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObservable avatarAvailableObservable
		{
			private get;
			set;
		}

		public IEnumerator WaitForAllCustomAvatars(List<string> inputAvatarNames, List<AvatarType> desiredAvatarType)
		{
			int avatarsLoadedCount = 0;
			for (int i = 0; i < inputAvatarNames.Count; i++)
			{
				RequestAvatarInternal(desiredAvatarType[i], inputAvatarNames[i], delegate(string name, AvatarType avatarType, Texture2D avatar)
				{
					Console.Log("request complete for avatar:" + name);
					avatarsLoadedCount++;
				});
			}
			while (avatarsLoadedCount < inputAvatarNames.Count)
			{
				yield return null;
			}
		}

		public void ForceRequestAvatar(AvatarType avatarType, string avatarName)
		{
			TaskRunner.get_Instance().Run(ForceRefreshAndRequestAvatar(avatarType, avatarName));
		}

		private IEnumerator ForceRefreshAndRequestAvatar(AvatarType avatarType, string avatarName)
		{
			IClearAvatarCacheRequest clearCacheRequest = SocialRequestFactory.Create<IClearAvatarCacheRequest>();
			if (avatarType == AvatarType.PlayerAvatar)
			{
				clearCacheRequest.Inject(new List<string>
				{
					avatarName
				}, null);
			}
			else
			{
				clearCacheRequest.Inject(null, new List<string>
				{
					avatarName
				});
			}
			yield return clearCacheRequest;
			RequestAvatar(avatarType, avatarName);
		}

		public void RequestAvatar(AvatarType avatarType, string avatarName)
		{
			RequestAvatarInternal(avatarType, avatarName, delegate
			{
			});
		}

		private void RequestAvatarInternal(AvatarType avatarType, string avatarName, Action<string, AvatarType, Texture2D> RequestResolvedCallback)
		{
			IMultiAvatarLoadRequest multiAvatarLoadRequest = SocialRequestFactory.Create<IMultiAvatarLoadRequest>();
			multiAvatarLoadRequest.Inject(new MultiAvatarRequestDependency(avatarName, avatarType));
			multiAvatarLoadRequest.SetAnswer(new ServiceAnswer<Texture2D>(delegate(Texture2D texture)
			{
				AvatarAvailableData avatarAvailableData2 = new AvatarAvailableData(avatarType, avatarName, texture);
				avatarAvailableObservable.Dispatch(ref avatarAvailableData2);
				RequestResolvedCallback(avatarName, avatarType, texture);
			}, delegate
			{
				AvatarAvailableData avatarAvailableData = new AvatarAvailableData(avatarType, avatarName, AvatarUtils.FailedToLoadTexture);
				avatarAvailableObservable.Dispatch(ref avatarAvailableData);
				RequestResolvedCallback(avatarName, avatarType, AvatarUtils.FailedToLoadTexture);
			})).Execute();
		}
	}
}
