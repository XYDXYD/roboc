using Authentication;
using Robocraft.GUI;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership.GUI.CustomGames
{
	internal class CustomGamePartyTeamsDataSource : DataSourceBase
	{
		private string _sessionleader = string.Empty;

		private CustomGameTeamChoice _team = CustomGameTeamChoice.NotSet;

		private List<string> _teamMembers = new List<string>();

		private List<string> _teamMembersDisplayName = new List<string>();

		private Dictionary<string, PlatoonMember.MemberStatus> _memberStatus = new Dictionary<string, PlatoonMember.MemberStatus>();

		private Dictionary<string, AvatarInfo> _teamAvatarInfos = new Dictionary<string, AvatarInfo>();

		private IServiceRequestFactory _serviceFactory;

		private bool _invalidateData;

		public CustomGamePartyTeamsDataSource(IServiceRequestFactory serviceRequestFactory)
		{
			_serviceFactory = serviceRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return _teamMembers.Count;
			}
			return 0;
		}

		public string GetMemberDisplayName(int memberIndex)
		{
			return _teamMembersDisplayName[memberIndex];
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			string text = _teamMembers[uniqueIdentifier1];
			if (typeof(T) == typeof(bool))
			{
				bool flag = _sessionleader.CompareTo(text) == 0;
				return (T)Convert.ChangeType(flag, typeof(T));
			}
			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType(text, typeof(T));
			}
			if (typeof(T) == typeof(AvatarInfo))
			{
				AvatarInfo value = _teamAvatarInfos[text];
				return (T)Convert.ChangeType(value, typeof(T));
			}
			if (typeof(T) == typeof(PlatoonMember.MemberStatus))
			{
				PlatoonMember.MemberStatus memberStatus = _memberStatus[text];
				return (T)Convert.ChangeType(memberStatus, typeof(T));
			}
			return default(T);
		}

		public void InvalidateCache()
		{
			_invalidateData = true;
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			TaskRunner.get_Instance().Run(RefreshDataWithEmumerator(OnSuccess, OnFailed));
		}

		public IEnumerator RefreshDataWithEmumerator(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			IEnumerator enumerator = RefreshData();
			yield return enumerator;
			object result = enumerator.Current;
			if (result == null)
			{
				TriggerAllDataChanged();
				OnSuccess();
			}
			else
			{
				OnFailed(result as ServiceBehaviour);
			}
		}

		public override IEnumerator RefreshData()
		{
			IRetrieveCustomGameSessionRequest request = _serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			if (_invalidateData)
			{
				request.ClearCache();
				_invalidateData = false;
			}
			TaskService<RetrieveCustomGameSessionRequestData> retrieveGameSessionTask = new TaskService<RetrieveCustomGameSessionRequestData>(request);
			yield return retrieveGameSessionTask;
			if (!retrieveGameSessionTask.succeeded)
			{
				yield return retrieveGameSessionTask.behaviour;
				yield break;
			}
			RetrieveCustomGameSessionRequestData result = retrieveGameSessionTask.result;
			if (result.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
			{
				ExtractData(result);
			}
			else
			{
				Console.Log("Party Teams data source could not refresh data as user is not in a custom game session");
			}
			yield return null;
		}

		private void ExtractData(RetrieveCustomGameSessionRequestData data)
		{
			if (_team == CustomGameTeamChoice.TeamA)
			{
				_teamMembers = new List<string>(data.Data.TeamAMembers);
				_teamMembersDisplayName = new List<string>(data.Data.TeamAMembersDisplayName);
			}
			else
			{
				_teamMembers = new List<string>(data.Data.TeamBMembers);
				_teamMembersDisplayName = new List<string>(data.Data.TeamBMembersDisplayName);
			}
			string username = User.Username;
			string displayName = User.DisplayName;
			if (_teamMembers.Contains(username))
			{
				_teamMembers.Remove(username);
				_teamMembers.Insert(0, username);
				_teamMembersDisplayName.Remove(displayName);
				_teamMembersDisplayName.Insert(0, displayName);
			}
			foreach (string teamMember in _teamMembers)
			{
				_memberStatus[teamMember] = data.Data.MemberStatus[teamMember];
			}
			_teamAvatarInfos = new Dictionary<string, AvatarInfo>(data.Data.TeamMemberAvatarInfos);
			_sessionleader = data.Data.SessionLeader;
			TriggerAllDataChanged();
		}

		public void SetTeam(CustomGameTeamChoice team)
		{
			_team = team;
		}

		public string GetSessionLeader()
		{
			return _sessionleader;
		}
	}
}
