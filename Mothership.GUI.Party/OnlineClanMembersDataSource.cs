using Authentication;
using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;

namespace Mothership.GUI.Party
{
	internal class OnlineClanMembersDataSource : AbstractOnlinePlayerListDataSource
	{
		private const int MAX_CLAN_MEMBERS = 50;

		private Platoon _party;

		private IServiceEventContainer _socialEventContainer;

		public OnlineClanMembersDataSource(ISocialRequestFactory socialRequestFactory, IServiceRequestFactory webRequestFactory, IServiceEventContainer socialEventContainer)
			: base(socialRequestFactory, webRequestFactory)
		{
			_socialEventContainer = socialEventContainer;
			RegisterToSocialEvents();
		}

		private void RegisterToSocialEvents()
		{
			_socialEventContainer.ListenTo<IClanMemberDataChangedEventListener, ClanMember[], ClanMemberDataChangedEventContent>(HandleOnClanMemberDataChanged);
			_socialEventContainer.ListenTo<IClanMemberJoinedEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoined);
			_socialEventContainer.ListenTo<IClanMemberLeftEventListener, ClanMember[], ClanMember>(HandleOnClanMemberLeft);
		}

		private void HandleOnClanMemberJoined(ClanMember[] clanMembers, ClanMember newMember)
		{
			if (newMember.IsOnline)
			{
				_playerList.Add(new InvitablePlayerData(newMember.Name, newMember.DisplayName, !_party.HasPlayer(newMember.Name), newMember.AvatarInfo));
				TriggerAllDataChanged();
			}
		}

		private void HandleOnClanMemberLeft(ClanMember[] clanMembers, ClanMember oldMember)
		{
			if (RemoveMember(oldMember.Name))
			{
				TriggerAllDataChanged();
			}
		}

		private bool RemoveMember(string name)
		{
			for (int i = 0; i < _playerList.Count; i++)
			{
				InvitablePlayerData invitablePlayerData = _playerList[i];
				if (invitablePlayerData.playerName == name)
				{
					_playerList.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private void HandleOnClanMemberDataChanged(ClanMember[] clanMembers, ClanMemberDataChangedEventContent data)
		{
			bool flag = false;
			if (data.IsOnline.HasValue && !data.IsOnline.Value && RemoveMember(data.UserName))
			{
				flag = true;
			}
			if (flag)
			{
				SortPlayers();
				TriggerAllDataChanged();
			}
		}

		public override IEnumerator RefreshData()
		{
			IGetMyClanInfoAndMembersRequest clanInfoRequest = _socialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>();
			TaskService<ClanInfoAndMembers> clanInfoTask = new TaskService<ClanInfoAndMembers>(clanInfoRequest);
			IGetPlatoonDataRequest getPlatoonRequest = _socialRequestFactory.Create<IGetPlatoonDataRequest>();
			TaskService<Platoon> getPlatoonTask = new TaskService<Platoon>(getPlatoonRequest);
			IRetrieveCustomGameSessionRequest getCustomGamePartyRequest = _webRequestFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> getCustomGamePartyTask = new TaskService<RetrieveCustomGameSessionRequestData>(getCustomGamePartyRequest);
			ParallelTaskCollection tasks = new ParallelTaskCollection();
			tasks.Add(clanInfoTask);
			tasks.Add(getPlatoonTask);
			tasks.Add(getCustomGamePartyTask);
			yield return tasks;
			if (!clanInfoTask.succeeded)
			{
				yield return clanInfoTask.behaviour;
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
			_party = getPlatoonTask.result;
			RetrieveCustomGameSessionRequestData customGameParty = getCustomGamePartyTask.result;
			ClanInfoAndMembers data = clanInfoTask.result;
			_playerList.Clear();
			if (data != null)
			{
				string username = User.Username;
				for (int i = 0; i < data.ClanMembers.Length; i++)
				{
					ClanMember clanMember = data.ClanMembers[i];
					if (clanMember.IsOnline && clanMember.Name != username)
					{
						_playerList.Add(new InvitablePlayerData(clanMember.Name, clanMember.DisplayName, !_party.HasPlayer(clanMember.Name) && !AbstractOnlinePlayerListDataSource.IsInCustomGameParty(clanMember.Name, customGameParty), clanMember.AvatarInfo));
					}
				}
				SortPlayers();
			}
			TriggerAllDataChanged();
		}
	}
}
