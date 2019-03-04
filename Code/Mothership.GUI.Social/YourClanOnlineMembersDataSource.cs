using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using Utility;

namespace Mothership.GUI.Social
{
	internal sealed class YourClanOnlineMembersDataSource : IDataSource
	{
		private bool _dataAvailable;

		private ISocialRequestFactory _socialRequestFactory;

		private int _membersOnline;

		public YourClanOnlineMembersDataSource(ISocialRequestFactory socialRequestFactory)
		{
			_socialRequestFactory = socialRequestFactory;
		}

		public int NumberOfDataItemsAvailable(int dimension)
		{
			if (_dataAvailable)
			{
				return 1;
			}
			return 0;
		}

		public T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			T result = default(T);
			if (typeof(T) == typeof(string))
			{
				result = (T)Convert.ChangeType(_membersOnline.ToString(), typeof(T));
			}
			return result;
		}

		public IEnumerator RefreshData()
		{
			bool finished = false;
			RefreshData(delegate
			{
				finished = true;
				_dataAvailable = true;
			}, delegate
			{
				Console.Log("failed to execute task " + GetType().Name);
				finished = true;
				_dataAvailable = false;
			});
			while (!finished)
			{
				yield return null;
			}
		}

		public void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
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

		private void OnSuccessResponse(ClanInfoAndMembers successData, Action onSuccess)
		{
			ClanMember[] clanMembers = successData.ClanMembers;
			CountOnlineMembers(clanMembers);
			onSuccess();
		}

		public void OnClanMemberDataChange(ClanMember[] clanMembers)
		{
			CountOnlineMembers(clanMembers);
		}

		private void CountOnlineMembers(ClanMember[] clanMembers)
		{
			_membersOnline = 0;
			for (int i = 0; i < clanMembers.Length; i++)
			{
				if (clanMembers[i].IsOnline && clanMembers[i].ClanMemberState == ClanMemberState.Accepted)
				{
					_membersOnline++;
				}
			}
		}
	}
}
