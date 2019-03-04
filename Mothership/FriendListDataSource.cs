using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class FriendListDataSource : DataSourceBase, IDisposable
	{
		public const int META_ONLINE_COUNT = -2;

		protected List<ExpandeableListHeaderData> _listHeaders = new List<ExpandeableListHeaderData>();

		protected List<Friend> _friendList;

		protected ISocialRequestFactory _socialRequestFactory;

		private Dictionary<string, Texture2D> _friendPlayerAvatars = new Dictionary<string, Texture2D>();

		private Platoon _party;

		private bool _receivedInvite;

		private bool _isInQueue;

		private IServiceRequestFactory _webRequestFactory;

		private IMultiAvatarLoader _avatarLoader;

		private AvatarAvailableObserver _avatarAvailableObserver;

		public unsafe FriendListDataSource(ISocialRequestFactory socialRequestFactory, IServiceRequestFactory webRequestFactory, IMultiAvatarLoader avatarLoader, AvatarAvailableObserver avatarAvailableObserver)
		{
			_socialRequestFactory = socialRequestFactory;
			_webRequestFactory = webRequestFactory;
			_avatarLoader = avatarLoader;
			_avatarAvailableObserver = avatarAvailableObserver;
			if (_avatarAvailableObserver != null)
			{
				_avatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (_friendList == null)
			{
				return 0;
			}
			if (dimension == 0)
			{
				return _friendList.Count;
			}
			return 0;
		}

		public unsafe void Dispose()
		{
			if (_avatarAvailableObserver != null)
			{
				_avatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarAvailableData)
		{
			if (avatarAvailableData.avatarType == AvatarType.PlayerAvatar && _friendList != null)
			{
				int num = 0;
				foreach (Friend friend in _friendList)
				{
					if (friend.AvatarInfo.UseCustomAvatar && friend.Name == avatarAvailableData.avatarName)
					{
						_friendPlayerAvatars[friend.Name] = avatarAvailableData.texture;
						TriggerDataItemChanged(num, 0);
						break;
					}
					num++;
				}
			}
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			T result = default(T);
			if (uniqueIdentifier1 == -2 && typeof(T) == typeof(string))
			{
				int num = 0;
				if (_friendList != null)
				{
					foreach (Friend friend2 in _friendList)
					{
						if (friend2.IsOnline)
						{
							num++;
						}
					}
				}
				return (T)Convert.ChangeType(num.ToString(), typeof(T));
			}
			if (_friendList == null)
			{
				return result;
			}
			ValidateData(uniqueIdentifier1, uniqueIdentifier2);
			if (typeof(T) == typeof(FriendListEntryData))
			{
				FriendListEntryData friendListEntryData = new FriendListEntryData();
				Friend friend = friendListEntryData.Friend = _friendList[uniqueIdentifier1];
				if (friend.InviteStatus == FriendInviteStatus.InviteSent)
				{
					friendListEntryData.StatusText = StringTableBase<StringTable>.Instance.GetString("strFriendInviteSentBrackets");
					friendListEntryData.StatusColourIndex = 2;
				}
				else if (friend.InviteStatus == FriendInviteStatus.InvitePending)
				{
					friendListEntryData.StatusText = string.Empty;
					friendListEntryData.StatusColourIndex = 0;
				}
				else
				{
					friendListEntryData.StatusText = StringTableBase<StringTable>.Instance.GetString((!friend.IsOnline) ? "strOfflineBrackets" : "strOnlineBrackets");
					friendListEntryData.StatusColourIndex = (friend.IsOnline ? 1 : 0);
				}
				if (friend.InviteStatus == FriendInviteStatus.InvitePending)
				{
					friendListEntryData.LeftButton = FriendListEntryData.FriendListButtonTypes.TICK;
					friendListEntryData.RightButton = FriendListEntryData.FriendListButtonTypes.CROSS;
				}
				else if (friend.InviteStatus == FriendInviteStatus.Accepted)
				{
					bool flag = false;
					bool flag2 = true;
					bool flag3 = false;
					if (_party != null && !string.IsNullOrEmpty(friend.Name))
					{
						if (_party.Size > 0)
						{
							flag = _party.HasPlayer(friend.Name);
							flag3 = (_party.Size >= 5);
						}
						flag2 = _party.GetIsPlatoonLeader();
					}
					if (friend.IsOnline && !_isInQueue && !flag && flag2 && !flag3 && !_receivedInvite)
					{
						friendListEntryData.RightButton = FriendListEntryData.FriendListButtonTypes.PLUS;
					}
					else
					{
						friendListEntryData.RightButton = FriendListEntryData.FriendListButtonTypes.NONE;
					}
				}
				else
				{
					friendListEntryData.LeftButton = FriendListEntryData.FriendListButtonTypes.NONE;
				}
				if (friend.InviteStatus == FriendInviteStatus.InviteSent)
				{
					friendListEntryData.RightButton = FriendListEntryData.FriendListButtonTypes.CROSS;
				}
				if (friend.AvatarInfo.UseCustomAvatar)
				{
					if (_friendPlayerAvatars.ContainsKey(friendListEntryData.FriendName))
					{
						friendListEntryData.FriendsPlayerAvatar = _friendPlayerAvatars[friendListEntryData.FriendName];
					}
					else
					{
						friendListEntryData.FriendsPlayerAvatar = AvatarUtils.StillLoadingTexture;
					}
				}
				result = (T)Convert.ChangeType(friendListEntryData, typeof(T));
			}
			else if (typeof(T) == typeof(ExpandeableListHeaderData))
			{
				ExpandeableListHeaderData value = _listHeaders[uniqueIdentifier1];
				result = (T)Convert.ChangeType(value, typeof(T));
			}
			else
			{
				Console.LogError("Unrecognized data type " + typeof(T).Name + " queried on " + GetType().Name);
			}
			return result;
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			TaskRunner.get_Instance().Run(RefreshDataWithEnumerator(OnSuccess, OnFailed));
		}

		public IEnumerator RefreshDataWithEnumerator(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
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

		public void SetPartyData(Platoon party)
		{
			_party = party;
		}

		public void SetPendingPartyInvite(bool receivedPending)
		{
			_receivedInvite = receivedPending;
		}

		public void SetIsInQueue(bool inQueue)
		{
			_isInQueue = inQueue;
		}

		public override IEnumerator RefreshData()
		{
			IGetFriendListRequest getFriendsRequest = _socialRequestFactory.Create<IGetFriendListRequest>();
			TaskService<GetFriendListResponse> getFriendsTask = new TaskService<GetFriendListResponse>(getFriendsRequest);
			IGetPlatoonDataRequest getPlatoonRequest = _socialRequestFactory.Create<IGetPlatoonDataRequest>();
			TaskService<Platoon> getPlatoonTask = new TaskService<Platoon>(getPlatoonRequest);
			ParallelTaskCollection tasks = new ParallelTaskCollection();
			tasks.Add(getFriendsTask);
			tasks.Add(getPlatoonTask);
			yield return tasks;
			if (!getFriendsTask.succeeded)
			{
				yield return getFriendsTask.behaviour;
				yield break;
			}
			if (!getPlatoonTask.succeeded)
			{
				yield return getPlatoonTask.behaviour;
				yield break;
			}
			_friendPlayerAvatars.Clear();
			List<Friend> friendList = new List<Friend>(getFriendsTask.result.friendsList);
			string[] userNames = new string[friendList.Count];
			for (int i = 0; i < userNames.Length; i++)
			{
				userNames[i] = friendList[i].Name;
			}
			_party = getPlatoonTask.result;
			_friendList = friendList;
			SortFriendList();
			ConstructHeadersTableBasedOnMembersList();
			if (_avatarLoader == null || _avatarAvailableObserver == null)
			{
				yield break;
			}
			for (int j = 0; j < userNames.Length; j++)
			{
				if (_friendList[j].AvatarInfo.UseCustomAvatar)
				{
					_avatarLoader.RequestAvatar(AvatarType.PlayerAvatar, _friendList[j].Name);
				}
				if (!string.IsNullOrEmpty(_friendList[j].ClanName))
				{
					_avatarLoader.RequestAvatar(AvatarType.ClanAvatar, _friendList[j].ClanName);
				}
			}
		}

		private void SortFriendList()
		{
			_friendList.Sort(friendComparer);
		}

		private int friendComparer(Friend a, Friend b)
		{
			int num = friendSortOrder(a) - friendSortOrder(b);
			return num + a.Name.CompareTo(b.Name);
		}

		private int friendSortOrder(Friend f)
		{
			int result = -1000;
			if (f.InviteStatus == FriendInviteStatus.InvitePending)
			{
				result = 1000;
			}
			if (f.InviteStatus == FriendInviteStatus.Accepted && f.IsOnline)
			{
				result = 2000;
			}
			if (f.InviteStatus == FriendInviteStatus.InviteSent)
			{
				result = 3000;
			}
			if (f.InviteStatus == FriendInviteStatus.Accepted && !f.IsOnline)
			{
				result = 4000;
			}
			return result;
		}

		private void ConstructHeadersTableBasedOnMembersList()
		{
			_listHeaders = new List<ExpandeableListHeaderData>();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			for (int i = 0; i < _friendList.Count; i++)
			{
				Friend friend = _friendList[i];
				if (!flag && friend.InviteStatus == FriendInviteStatus.InvitePending)
				{
					_listHeaders.Add(new ExpandeableListHeaderData(i, expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strFriendRequests"), 3));
					flag = true;
				}
				if (!flag2 && friend.InviteStatus == FriendInviteStatus.Accepted && friend.IsOnline)
				{
					_listHeaders.Add(new ExpandeableListHeaderData(i, expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strOnline"), 2));
					flag2 = true;
				}
				if (!flag3 && friend.InviteStatus == FriendInviteStatus.InviteSent)
				{
					_listHeaders.Add(new ExpandeableListHeaderData(i, expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strInvitesYouSent"), 1));
					flag3 = true;
				}
				if (!flag4 && friend.InviteStatus == FriendInviteStatus.Accepted && !friend.IsOnline)
				{
					_listHeaders.Add(new ExpandeableListHeaderData(i, expandedStatus_: true, StringTableBase<StringTable>.Instance.GetString("strOffline"), 0));
					flag4 = true;
				}
			}
		}
	}
}
