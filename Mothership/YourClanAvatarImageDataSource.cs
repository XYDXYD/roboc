using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class YourClanAvatarImageDataSource : DataSourceBase, IDisposable
	{
		private string _yourClanName;

		private Texture2D _clanAvatarImage;

		private string _searchCriteria = string.Empty;

		private ClanViewMode _clanDataMode = ClanViewMode.NoClan;

		private readonly ISocialRequestFactory _socialRequestFactory;

		private readonly IServiceEventContainer _socialEventContainer;

		private PresetAvatarMap _presetAvatarMap;

		private int clanAvatarNumber = -1;

		private IMultiAvatarLoader _avatarLoader;

		private AvatarAvailableObserver _avatarAvailableObserver;

		public event Action onDataChanged;

		public unsafe YourClanAvatarImageDataSource(ISocialRequestFactory socialRequestFactory, IServiceEventContainer socialEventContainer, IMultiAvatarLoader avatarLoader, AvatarAvailableObserver avatarAvailableObserver, PresetAvatarMap presetAvatarMap)
		{
			_socialRequestFactory = socialRequestFactory;
			_socialEventContainer = socialEventContainer;
			_avatarLoader = avatarLoader;
			_presetAvatarMap = presetAvatarMap;
			_avatarAvailableObserver = avatarAvailableObserver;
			_avatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_socialEventContainer.ListenTo<IClanDataChangedEventListener, ClanInfo>(HandleOnClanDataChanged);
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return 1;
			}
			return 0;
		}

		public unsafe void Dispose()
		{
			_avatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnHandleAvatarAvailable(ref AvatarAvailableData avatarAvailable)
		{
		}

		private void HandleOnClanDataChanged(ClanInfo clanInfo)
		{
			int avatarId = clanInfo.ClanAvatarNumber;
			_clanAvatarImage = _presetAvatarMap.GetPresetAvatar(avatarId);
		}

		public void SetSearchTerm(ClanViewMode clanDataMode, string searchCriteria)
		{
			_clanDataMode = clanDataMode;
			_searchCriteria = searchCriteria;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			ValidateData(uniqueIdentifier1, uniqueIdentifier2);
			T result = default(T);
			if (typeof(T) == typeof(Texture2D))
			{
				try
				{
					return (T)Convert.ChangeType(_clanAvatarImage, typeof(T));
				}
				catch
				{
					return default(T);
				}
			}
			return result;
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			_clanAvatarImage = null;
			if (_clanDataMode == ClanViewMode.YourClan)
			{
				IGetMyClanInfoRequest getMyClanInfoRequest = _socialRequestFactory.Create<IGetMyClanInfoRequest>();
				getMyClanInfoRequest.SetAnswer(new ServiceAnswer<ClanInfo>(delegate(ClanInfo clanInfo)
				{
					_yourClanName = clanInfo.ClanName;
					int avatarId2 = clanInfo.ClanAvatarNumber;
					_clanAvatarImage = _presetAvatarMap.GetPresetAvatar(avatarId2);
					OnSuccess();
				}, delegate(ServiceBehaviour behaviour)
				{
					OnFailed(behaviour);
				})).Execute();
			}
			if (_clanDataMode == ClanViewMode.AnotherClan)
			{
				IGetClanInfoAndMembersRequest getClanInfoAndMembersRequest = _socialRequestFactory.Create<IGetClanInfoAndMembersRequest>();
				getClanInfoAndMembersRequest.Inject(_searchCriteria);
				getClanInfoAndMembersRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(delegate(ClanInfoAndMembers clanInfo)
				{
					_yourClanName = clanInfo.ClanInfo.ClanName;
					int avatarId = clanInfo.ClanInfo.ClanAvatarNumber;
					_clanAvatarImage = _presetAvatarMap.GetPresetAvatar(avatarId);
					OnSuccess();
				}, delegate(ServiceBehaviour behaviour)
				{
					OnFailed(behaviour);
				})).Execute();
			}
			if (_clanDataMode == ClanViewMode.NoClan)
			{
				OnSuccess();
			}
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
	}
}
