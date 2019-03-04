using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal class PlayerHeadersListDataSource : DataSourceBase
	{
		private const int MAX_CLAN_MEMBERS = 50;

		private string _searchCriteria = string.Empty;

		private List<ExpandeableListHeaderData> _listHeaders = new List<ExpandeableListHeaderData>();

		private ClanMember[] _memberInfo;

		private ClanViewMode _clanDataMode = ClanViewMode.NoClan;

		private ISocialRequestFactory _socialRequestFactory;

		private IServiceEventContainer _socialEventContainer;

		public event Action onDataChanged;

		public PlayerHeadersListDataSource(ISocialRequestFactory socialRequestFactory, IServiceEventContainer socialEventContainer)
		{
			_socialRequestFactory = socialRequestFactory;
			_socialEventContainer = socialEventContainer;
			RegisterToSocialEvent();
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (_memberInfo == null)
			{
				return 0;
			}
			if (dimension == 0)
			{
				return _listHeaders.Count;
			}
			return 0;
		}

		private void RegisterToSocialEvent()
		{
			_socialEventContainer.ListenTo<IClanMemberJoinedEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoinedOrLeft);
			_socialEventContainer.ListenTo<IClanMemberLeftEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoinedOrLeft);
			_socialEventContainer.ListenTo<IClanMemberDataChangedEventListener, ClanMember[], ClanMemberDataChangedEventContent>(HandleOnClanMemberDataChanged);
		}

		private void HandleOnClanMemberDataChanged(ClanMember[] clanMembers, ClanMemberDataChangedEventContent data)
		{
			UpdateClanMembersArray(clanMembers);
		}

		private void HandleOnClanMemberJoinedOrLeft(ClanMember[] clanMembers, ClanMember member)
		{
			UpdateClanMembersArray(clanMembers);
		}

		private void UpdateClanMembersArray(ClanMember[] clanMembers)
		{
			_memberInfo = new ClanMember[clanMembers.Length];
			Array.Copy(clanMembers, _memberInfo, clanMembers.Length);
			FilterMembersList();
			ConstructHeadersTableBasedOnMembersList();
			SafeEvent.SafeRaise(this.onDataChanged);
		}

		public void SetSearchTerm(ClanViewMode clanDataMode, string searchCriteria)
		{
			_clanDataMode = clanDataMode;
			_searchCriteria = searchCriteria;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (_memberInfo == null)
			{
				return default(T);
			}
			ValidateData(uniqueIdentifier1, uniqueIdentifier2);
			T result = default(T);
			if (typeof(T) == typeof(ExpandeableListHeaderData))
			{
				ExpandeableListHeaderData value = _listHeaders[uniqueIdentifier1];
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
			if (_clanDataMode == ClanViewMode.YourClan)
			{
				IGetMyClanInfoAndMembersRequest getMyClanInfoAndMembersRequest = _socialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>();
				getMyClanInfoAndMembersRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(delegate(ClanInfoAndMembers successData)
				{
					OnSuccessResponse(successData, OnSuccess);
				}, delegate(ServiceBehaviour behaviour)
				{
					OnFailed(behaviour);
				})).Execute();
			}
			if (_clanDataMode == ClanViewMode.AnotherClan)
			{
				IGetClanInfoAndMembersRequest getClanInfoAndMembersRequest = _socialRequestFactory.Create<IGetClanInfoAndMembersRequest>();
				getClanInfoAndMembersRequest.Inject(_searchCriteria);
				getClanInfoAndMembersRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(delegate(ClanInfoAndMembers successData)
				{
					OnSuccessResponse(successData, OnSuccess);
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

		private void OnSuccessResponse(ClanInfoAndMembers data, Action FinishedCallback)
		{
			_memberInfo = new ClanMember[data.ClanMembers.Length];
			Array.Copy(data.ClanMembers, _memberInfo, data.ClanMembers.Length);
			FilterMembersList();
			ConstructHeadersTableBasedOnMembersList();
			FinishedCallback();
			TriggerAllDataChanged();
		}

		private void FilterMembersList()
		{
			Array.Sort(_memberInfo, new PlayerCategorySorter());
		}

		private void ConstructHeadersTableBasedOnMembersList()
		{
			_listHeaders.Clear();
			_listHeaders.Add(new ExpandeableListHeaderData(0, expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strClanPlayerListHeaderOnline"), 1));
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			for (num = 0; num < _memberInfo.Length; num++)
			{
				if (flag && flag2)
				{
					break;
				}
				if (!_memberInfo[num].IsOnline && !flag)
				{
					_listHeaders.Add(new ExpandeableListHeaderData(num, expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strClanPlayerListHeaderOffline"), 0));
					flag = true;
				}
				if (_memberInfo[num].ClanMemberState == ClanMemberState.Invited && !flag2)
				{
					_listHeaders.Add(new ExpandeableListHeaderData(num, expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strClanPlayerListHeaderPendingInvite"), 3));
					flag2 = true;
				}
			}
			if (!flag)
			{
				_listHeaders.Add(new ExpandeableListHeaderData(_memberInfo.GetLength(0), expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strClanPlayerListHeaderOffline"), 0));
			}
		}
	}
}
