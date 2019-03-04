using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal class InviteesHeadersListDataSource : DataSourceBase
	{
		private readonly List<ExpandeableListHeaderData> _listHeaders = new List<ExpandeableListHeaderData>();

		private ClanInvite[] _invitations;

		private readonly ISocialRequestFactory _socialRequestFactory;

		public InviteesHeadersListDataSource(ISocialRequestFactory socialRequestFactory)
		{
			_socialRequestFactory = socialRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				if (_invitations == null)
				{
					return 0;
				}
				return _listHeaders.Count;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (_invitations == null)
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
					Console.LogError("Error: expected PlayerHeadersListDataSource to be populated with ExpandeableListHeaderData[]");
					return default(T);
				}
			}
			return result;
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			IGetClanInvitesRequest getClanInvitesRequest = _socialRequestFactory.Create<IGetClanInvitesRequest>();
			getClanInvitesRequest.SetAnswer(new ServiceAnswer<ClanInvite[]>(delegate(ClanInvite[] successData)
			{
				OnSuccessResponse(successData, OnSuccess);
				TriggerAllDataChanged();
			}, delegate(ServiceBehaviour behaviour)
			{
				OnFailed(behaviour);
			})).Execute();
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

		private void OnSuccessResponse(ClanInvite[] invites, Action FinishedCallback)
		{
			_invitations = (ClanInvite[])invites.Clone();
			ConstructHeadersTableBasedOnMembersList();
			FinishedCallback();
		}

		private void ConstructHeadersTableBasedOnMembersList()
		{
			_listHeaders.Clear();
		}
	}
}
