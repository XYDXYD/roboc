using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonMemberAddedEventListener : SocialEventListener, IPlatoonMemberAddedEventListener, IServiceEventListener, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonMemberAdded;

		protected override void ParseData(EventData eventData)
		{
			Dictionary<byte, object> parameters = eventData.Parameters;
			PlatoonMember parameter = parameters.GetParameter<PlatoonMember>(SocialParameterCode.PlatoonMember);
			CacheDTO.platoon.AddMember(parameter);
			Invoke();
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
		}
	}
}
