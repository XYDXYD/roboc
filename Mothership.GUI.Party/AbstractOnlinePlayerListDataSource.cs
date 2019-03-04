using Robocraft.GUI;
using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mothership.GUI.Party
{
	internal abstract class AbstractOnlinePlayerListDataSource : DataSourceBase
	{
		protected List<InvitablePlayerData> _playerList = new List<InvitablePlayerData>();

		protected ISocialRequestFactory _socialRequestFactory;

		protected IServiceRequestFactory _webRequestFactory;

		[CompilerGenerated]
		private static Comparison<InvitablePlayerData> _003C_003Ef__mg_0024cache0;

		public AbstractOnlinePlayerListDataSource(ISocialRequestFactory socialRequestFactory, IServiceRequestFactory webRequestFactory)
		{
			_socialRequestFactory = socialRequestFactory;
			_webRequestFactory = webRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return _playerList.Count;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			ValidateData(uniqueIdentifier1, uniqueIdentifier2);
			T result = default(T);
			if (typeof(T) == typeof(InvitablePlayerData))
			{
				InvitablePlayerData invitablePlayerData = _playerList[uniqueIdentifier1];
				result = (T)Convert.ChangeType(invitablePlayerData, typeof(T));
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
				TriggerAllDataChanged();
			}
			else
			{
				OnFailed(result as ServiceBehaviour);
			}
		}

		protected static bool IsInCustomGameParty(string playerName, RetrieveCustomGameSessionRequestData customGame)
		{
			if (customGame == null || customGame.Data == null)
			{
				return false;
			}
			return customGame.Data.TeamAMembers.Contains(playerName) || customGame.Data.TeamBMembers.Contains(playerName);
		}

		protected void SortPlayers()
		{
			_playerList.Sort(comparePlayers);
		}

		private static int comparePlayers(InvitablePlayerData a, InvitablePlayerData b)
		{
			return a.playerName.CompareTo(b.playerName);
		}
	}
}
