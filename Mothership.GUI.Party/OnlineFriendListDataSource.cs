using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace Mothership.GUI.Party
{
	internal class OnlineFriendListDataSource : AbstractOnlinePlayerListDataSource
	{
		public OnlineFriendListDataSource(ISocialRequestFactory socialRequestFactory, IServiceRequestFactory webRequestFactory)
			: base(socialRequestFactory, webRequestFactory)
		{
		}

		public override IEnumerator RefreshData()
		{
			IGetFriendListRequest getFriendsRequest = _socialRequestFactory.Create<IGetFriendListRequest>();
			TaskService<GetFriendListResponse> getFriendsTask = new TaskService<GetFriendListResponse>(getFriendsRequest);
			IGetPlatoonDataRequest getPlatoonRequest = _socialRequestFactory.Create<IGetPlatoonDataRequest>();
			TaskService<Platoon> getPlatoonTask = new TaskService<Platoon>(getPlatoonRequest);
			IRetrieveCustomGameSessionRequest getCustomGamePartyRequest = _webRequestFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> getCustomGamePartyTask = new TaskService<RetrieveCustomGameSessionRequestData>(getCustomGamePartyRequest);
			ParallelTaskCollection tasks = new ParallelTaskCollection();
			tasks.Add(getFriendsTask);
			tasks.Add(getPlatoonTask);
			tasks.Add(getCustomGamePartyTask);
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
			if (!getCustomGamePartyTask.succeeded)
			{
				yield return getCustomGamePartyTask.behaviour;
				yield break;
			}
			IList<Friend> src = getFriendsTask.result.friendsList;
			List<InvitablePlayerData> friendList = new List<InvitablePlayerData>(src.Count);
			Platoon party = getPlatoonTask.result;
			RetrieveCustomGameSessionRequestData customGameParty = getCustomGamePartyTask.result;
			for (int i = 0; i < src.Count; i++)
			{
				Friend friend = src[i];
				if (friend.IsOnline && friend.InviteStatus == FriendInviteStatus.Accepted)
				{
					InvitablePlayerData item = new InvitablePlayerData(friend.Name, friend.DisplayName, !party.HasPlayer(friend.Name) && !AbstractOnlinePlayerListDataSource.IsInCustomGameParty(friend.Name, customGameParty), friend.AvatarInfo);
					friendList.Add(item);
				}
			}
			_playerList = friendList;
			SortPlayers();
			TriggerAllDataChanged();
		}
	}
}
