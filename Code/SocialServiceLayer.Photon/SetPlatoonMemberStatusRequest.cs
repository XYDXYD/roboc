using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class SetPlatoonMemberStatusRequest : SocialRequest, ISetPlatoonMemberStatusRequest, IServiceRequest<SetPlatoonMemberStatusDependency>, IAnswerOnComplete, IServiceRequest
	{
		private SetPlatoonMemberStatusDependency _dependency;

		protected override byte OperationCode => 17;

		public SetPlatoonMemberStatusRequest()
			: base("strRobocloudError", "strSetPlatMemStatusError", 0)
		{
		}

		public void Inject(SetPlatoonMemberStatusDependency dependecy)
		{
			_dependency = dependecy;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			CacheDTO.platoon.SetMemberStatus(_dependency.UserName, _dependency.Status);
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[3] = _dependency.Status;
			return val;
		}
	}
}
