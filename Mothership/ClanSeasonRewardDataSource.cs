using Authentication;
using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class ClanSeasonRewardDataSource : DataSourceBase, IDisposable
	{
		private const int SEASONS_START_MONTH = 10;

		private const int SEASONS_START_YEAR = 2016;

		private IMultiAvatarLoader _avatarLoader;

		private AvatarAvailableObserver _avatarAvailableObserver;

		private string _averageSeasonExperience = "4815162342";

		private Texture2D _clanAvatar;

		private string _clanName = string.Empty;

		private string _personalSeasonExperience = "4815162342";

		private int _robitsReward = -1;

		private int _seasonNumber;

		private ISocialRequestFactory _socialRequestFactory;

		private string _totalSeasonExperience = "4815162342";

		public unsafe ClanSeasonRewardDataSource(ISocialRequestFactory requestFactory, IMultiAvatarLoader avatarLoader, AvatarAvailableObserver avatarAvailableObserver)
		{
			_socialRequestFactory = requestFactory;
			_avatarLoader = avatarLoader;
			_avatarAvailableObserver = avatarAvailableObserver;
			_avatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnAvatarDataAvailable(ref AvatarAvailableData availableData)
		{
			if (_clanName == availableData.avatarName && availableData.avatarType == AvatarType.ClanAvatar)
			{
				_clanAvatar = availableData.texture;
				TriggerDataItemChanged(2, 0);
			}
		}

		public unsafe void Dispose()
		{
			_avatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0 && _robitsReward > 0)
			{
				return 1;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (typeof(T) == typeof(string))
			{
				switch (uniqueIdentifier1)
				{
				case 0:
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(Localization.Get("strSeasonText", true));
					stringBuilder.Append(" ");
					stringBuilder.Append(_seasonNumber);
					return (T)Convert.ChangeType(stringBuilder.ToString(), typeof(T));
				}
				case 3:
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(Localization.Get("strRobits", true));
					stringBuilder.Append(" - ");
					stringBuilder.Append(_robitsReward);
					return (T)Convert.ChangeType(stringBuilder.ToString(), typeof(T));
				}
				case 4:
					return (T)Convert.ChangeType(_personalSeasonExperience, typeof(T));
				case 5:
					return (T)Convert.ChangeType(_averageSeasonExperience, typeof(T));
				case 6:
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(Localization.Get("strClanTotalXp", true));
					stringBuilder.Append(" - ");
					stringBuilder.Append(_totalSeasonExperience);
					return (T)Convert.ChangeType(stringBuilder.ToString(), typeof(T));
				}
				case 1:
					return (T)Convert.ChangeType(_clanName, typeof(T));
				case 7:
					return (T)Convert.ChangeType(_seasonNumber.ToString(), typeof(T));
				}
			}
			else if (typeof(T) == typeof(Texture2D) && uniqueIdentifier1 == 2)
			{
				return (T)Convert.ChangeType(_clanAvatar, typeof(T));
			}
			return default(T);
		}

		public override IEnumerator RefreshData()
		{
			bool finished = false;
			RefreshData(delegate
			{
				finished = true;
			}, delegate
			{
				Console.Log("failed to execute task " + GetType().Name);
				finished = true;
			});
			while (!finished)
			{
				yield return null;
			}
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			IFetchSeasonRewardsRequest fetchSeasonRewardsRequest = _socialRequestFactory.Create<IFetchSeasonRewardsRequest>();
			fetchSeasonRewardsRequest.Inject(new FetchSeasonRewardsDependancy(User.Username));
			fetchSeasonRewardsRequest.SetAnswer(new ServiceAnswer<FetchSeasonRewardsResponse>(delegate(FetchSeasonRewardsResponse data)
			{
				OnRewardsFetched(data, OnSuccess);
			}, OnFailed)).Execute();
		}

		private static int GetSeasonNumber(int rewardMonth, int rewardYear)
		{
			return (rewardYear - 2016) * 12 + rewardMonth - 10;
		}

		private void OnRewardsFetched(FetchSeasonRewardsResponse rewardData, Action onSuccess)
		{
			if (rewardData.robitsReward > 0 || rewardData.totalSeasonXPForThisPlayer > 0)
			{
				_avatarLoader.RequestAvatar(AvatarType.ClanAvatar, rewardData.clanName);
			}
			OnAllDataFetched(rewardData, onSuccess);
		}

		private void OnAllDataFetched(FetchSeasonRewardsResponse rewardData, Action onSuccess)
		{
			if (rewardData != null && !rewardData.hasClaimedAllRewards)
			{
				_robitsReward = rewardData.robitsReward;
				_seasonNumber = GetSeasonNumber(rewardData.seasonMonth, rewardData.seasonYear);
				_clanName = rewardData.clanName;
				_personalSeasonExperience = rewardData.totalSeasonXPForThisPlayer.ToString();
				_averageSeasonExperience = rewardData.averageSeasonExperienceForEveryoneInClan.ToString();
				_totalSeasonExperience = rewardData.clansTotalExperience.ToString();
			}
			onSuccess();
			TriggerAllDataChanged();
		}
	}
}
