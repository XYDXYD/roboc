using Authentication;
using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class PlayerListDataSource : DataSourceBase, IDisposable
	{
		private const int MAX_CLAN_MEMBERS = 50;

		private ISocialRequestFactory _socialRequestFactory;

		private IServiceRequestFactory _webRequestFactory;

		private string _searchCriteria = string.Empty;

		private ClanViewMode _clanDataMode = ClanViewMode.NoClan;

		private ClanMemberWithContextInfo[] _memberInfo = new ClanMemberWithContextInfo[0];

		private Platoon _party;

		private IComparer<IClanMemberListSortingData> playerCategorySorter = new PlayerCategorySorter();

		private ClanMemberRank _yourMemberRank = ClanMemberRank.Officer;

		private IServiceEventContainer _socialEventContainer;

		private IMultiAvatarLoader _avatarLoader;

		private AvatarAvailableObserver _avatarAvailableObserver;

		public PlayerListDataSource(ISocialRequestFactory socialRequestFactory, IServiceRequestFactory webRequestFactory, IServiceEventContainer socialEventContainer, IMultiAvatarLoader avatarLoader_, AvatarAvailableObserver avatarAvailableObserver_)
		{
			_socialRequestFactory = socialRequestFactory;
			_socialEventContainer = socialEventContainer;
			_webRequestFactory = webRequestFactory;
			_avatarAvailableObserver = avatarAvailableObserver_;
			_avatarLoader = avatarLoader_;
			RegisterToSocialEvents();
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return _memberInfo.GetLength(0);
			}
			return 0;
		}

		public unsafe void Dispose()
		{
			_avatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void RegisterToSocialEvents()
		{
			_socialEventContainer.ListenTo<IClanDataChangedEventListener, ClanInfo>(HandleOnClanDataChanged);
			_socialEventContainer.ListenTo<IClanMemberXPChangedEventListener, ClanMemberXPChangedEventContent>(HandleOnClanMemberXPChanged);
			_socialEventContainer.ListenTo<IClanMemberDataChangedEventListener, ClanMember[], ClanMemberDataChangedEventContent>(HandleOnClanMemberDataChanged);
			_socialEventContainer.ListenTo<IClanMemberJoinedEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoinedOrLeft);
			_socialEventContainer.ListenTo<IClanMemberLeftEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoinedOrLeft);
			_avatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnHandleAvatarAvailable(ref AvatarAvailableData data)
		{
			if (data.avatarType != 0)
			{
				return;
			}
			int num = 0;
			ClanMemberWithContextInfo[] memberInfo = _memberInfo;
			int num2 = 0;
			ClanMemberWithContextInfo clanMemberWithContextInfo;
			while (true)
			{
				if (num2 < memberInfo.Length)
				{
					clanMemberWithContextInfo = memberInfo[num2];
					if (clanMemberWithContextInfo.Member.Name == data.avatarName)
					{
						break;
					}
					num++;
					num2++;
					continue;
				}
				return;
			}
			clanMemberWithContextInfo.PlayerAvatarTexture = data.texture;
			TriggerDataItemChanged(num, 0);
		}

		private void HandleOnClanDataChanged(ClanInfo clanInfo)
		{
			TriggerAllDataChanged();
		}

		private void HandleOnClanMemberJoinedOrLeft(ClanMember[] clanMembers, ClanMember newMember)
		{
			_memberInfo = new ClanMemberWithContextInfo[clanMembers.Length];
			for (int i = 0; i < clanMembers.Length; i++)
			{
				_memberInfo[i] = new ClanMemberWithContextInfo(clanMembers[i], _yourMemberRank, RequestingYourClan_: true);
			}
			FilterMembersList();
			TriggerAllDataChanged();
		}

		private void HandleOnClanMemberDataChanged(ClanMember[] clanMembers, ClanMemberDataChangedEventContent data)
		{
			for (int i = 0; i < _memberInfo.Length; i++)
			{
				ClanMember member = _memberInfo[i].Member;
				if (member.Name == data.UserName)
				{
					if (member.AvatarInfo.UseCustomAvatar)
					{
						_avatarLoader.ForceRequestAvatar(AvatarType.PlayerAvatar, data.UserName);
					}
					if (data.IsOnline.HasValue)
					{
						member.IsOnline = data.IsOnline.Value;
					}
					if (data.ClanMemberRank.HasValue)
					{
						member.ClanMemberRank = data.ClanMemberRank.Value;
					}
					break;
				}
			}
			FilterMembersList();
			TriggerAllDataChanged();
		}

		private void HandleOnClanMemberXPChanged(ClanMemberXPChangedEventContent data)
		{
			for (int i = 0; i < _memberInfo.Length; i++)
			{
				ClanMember member = _memberInfo[i].Member;
				if (member.Name == data.memberName)
				{
					member.SeasonXP = data.newXPValue;
					break;
				}
			}
			FilterMembersList();
			TriggerAllDataChanged();
		}

		public void SetSearchTerm(ClanViewMode clanDataMode, string searchCriteria)
		{
			_searchCriteria = searchCriteria;
			_clanDataMode = clanDataMode;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (_memberInfo.Length == 0)
			{
				return default(T);
			}
			T result = default(T);
			if (typeof(T) == typeof(ClanMemberWithContextInfo))
			{
				ClanMemberWithContextInfo value = _memberInfo[uniqueIdentifier1];
				try
				{
					return (T)Convert.ChangeType(value, typeof(T));
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
			if (_clanDataMode == ClanViewMode.NoClan)
			{
				OnSuccess();
			}
			TaskRunner.get_Instance().Run(RefreshDataWithEnumerator(OnSuccess, OnFailed));
		}

		public override IEnumerator RefreshData()
		{
			IEnumerator en = RefreshData_Internal();
			yield return en;
			if (en.Current is ServiceBehaviour)
			{
				ServiceBehaviour behaviour = (ServiceBehaviour)en.Current;
				ErrorWindow.ShowServiceErrorWindow(behaviour);
			}
		}

		private IEnumerator RefreshData_Internal()
		{
			if (_clanDataMode == ClanViewMode.NoClan)
			{
				yield break;
			}
			IAnswerOnComplete<ClanInfoAndMembers> clanInfoRequest;
			if (_clanDataMode == ClanViewMode.YourClan)
			{
				clanInfoRequest = _socialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>();
			}
			else
			{
				IGetClanInfoAndMembersRequest getClanInfoAndMembersRequest = _socialRequestFactory.Create<IGetClanInfoAndMembersRequest>();
				getClanInfoAndMembersRequest.Inject(_searchCriteria);
				clanInfoRequest = getClanInfoAndMembersRequest;
			}
			TaskService<ClanInfoAndMembers> clanInfoTask = new TaskService<ClanInfoAndMembers>(clanInfoRequest);
			yield return clanInfoTask;
			if (!clanInfoTask.succeeded)
			{
				yield return clanInfoTask.behaviour;
				yield break;
			}
			IGetPlatoonDataRequest platoonRequest = _socialRequestFactory.Create<IGetPlatoonDataRequest>();
			TaskService<Platoon> platoonTask = new TaskService<Platoon>(platoonRequest);
			yield return platoonTask;
			if (!platoonTask.succeeded)
			{
				yield return platoonTask.behaviour;
				yield break;
			}
			_party = platoonTask.result;
			ClanInfoAndMembers clanInfo = clanInfoTask.result;
			string[] userNames = new string[clanInfo.ClanMembers.Length];
			for (int i = 0; i < clanInfo.ClanMembers.Length; i++)
			{
				userNames[i] = clanInfo.ClanMembers[i].Name;
			}
			OnSuccessResponse(clanInfo);
		}

		private IEnumerator RefreshDataWithEnumerator(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			IEnumerator enumerator = RefreshData();
			yield return enumerator;
			object result = enumerator.Current;
			if (result == null)
			{
				OnSuccess();
			}
			else
			{
				OnFailed(result as ServiceBehaviour);
			}
		}

		public void SetPartyData(Platoon platoon)
		{
			_party = platoon;
		}

		public Platoon GetPartyData()
		{
			return _party;
		}

		private void GetPartyData(Action<Platoon> onSuccess, Action<ServiceBehaviour> onError)
		{
			IGetPlatoonDataRequest getPlatoonDataRequest = _socialRequestFactory.Create<IGetPlatoonDataRequest>();
			getPlatoonDataRequest.SetAnswer(new ServiceAnswer<Platoon>(onSuccess, onError));
			getPlatoonDataRequest.Execute();
		}

		private void OnSuccessResponse(ClanInfoAndMembers data)
		{
			_yourMemberRank = ClanMemberRank.Member;
			bool flag = _clanDataMode == ClanViewMode.YourClan;
			if (flag)
			{
				for (int i = 0; i < data.ClanMembers.Length; i++)
				{
					if (data.ClanMembers[i].Name == User.Username)
					{
						_yourMemberRank = data.ClanMembers[i].ClanMemberRank;
					}
				}
			}
			_memberInfo = new ClanMemberWithContextInfo[data.ClanMembers.Length];
			for (int j = 0; j < data.ClanMembers.Length; j++)
			{
				ClanMember clanMember = data.ClanMembers[j];
				_memberInfo[j] = new ClanMemberWithContextInfo(clanMember, _yourMemberRank, flag);
				if (_memberInfo[j].Member.AvatarInfo.UseCustomAvatar)
				{
					_memberInfo[j].PlayerAvatarTexture = AvatarUtils.StillLoadingTexture;
					_avatarLoader.RequestAvatar(AvatarType.PlayerAvatar, clanMember.Name);
				}
			}
			FilterMembersList();
			TriggerAllDataChanged();
		}

		private void FilterMembersList()
		{
			Array.Sort(_memberInfo, playerCategorySorter);
		}
	}
}
