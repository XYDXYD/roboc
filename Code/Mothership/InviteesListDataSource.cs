using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class InviteesListDataSource : DataSourceBase
	{
		private ClanInvite[] _invitations;

		private readonly ISocialRequestFactory _socialRequestFactory;

		public InviteesListDataSource(ISocialRequestFactory socialRequestFactory)
		{
			_socialRequestFactory = socialRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (_invitations == null)
			{
				return 0;
			}
			if (dimension == 0)
			{
				return _invitations.GetLength(0);
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (_invitations == null)
			{
				return default(T);
			}
			T result = default(T);
			ValidateData(uniqueIdentifier1, uniqueIdentifier2);
			if (typeof(T) == typeof(ClanInvite))
			{
				ClanInvite value = _invitations[uniqueIdentifier1];
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
			IGetClanInvitesRequest getClanInvitesRequest = _socialRequestFactory.Create<IGetClanInvitesRequest>();
			getClanInvitesRequest.SetAnswer(new ServiceAnswer<ClanInvite[]>(delegate(ClanInvite[] successData)
			{
				OnSuccessResponse(successData, OnSuccess);
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
			FinishedCallback();
			TriggerAllDataChanged();
		}
	}
}
