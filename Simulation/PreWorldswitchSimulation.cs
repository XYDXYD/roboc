using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;

namespace Simulation
{
	internal class PreWorldswitchSimulation : IInitialize
	{
		[Inject]
		internal IDispatchWorldSwitching DispatchWorldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory SocialRequestFactory
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			ParallelTaskCollection val = new ParallelTaskCollection();
			IGetFriendListRequest getFriendListRequest = SocialRequestFactory.Create<IGetFriendListRequest>();
			getFriendListRequest.ForceRefresh = true;
			IGetMyClanInfoAndMembersRequest getMyClanInfoAndMembersRequest = SocialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>();
			getMyClanInfoAndMembersRequest.ForceRefresh = true;
			IGetClanInvitesRequest getClanInvitesRequest = SocialRequestFactory.Create<IGetClanInvitesRequest>();
			getClanInvitesRequest.ForceRefresh = true;
			val.Add(getFriendListRequest.AsTask());
			val.Add(getMyClanInfoAndMembersRequest);
			val.Add(getClanInvitesRequest.AsTask());
			DispatchWorldSwitching.OnWorldIsSwitching.Add((IEnumerator)val);
		}
	}
}
