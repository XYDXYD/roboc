using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer.Photon
{
	internal class LeavePlatoonRequest : SocialRequest, ILeavePlatoonRequest, IServiceRequest, IAnswerOnComplete, ITask, IAbstractTask
	{
		private string _oldPlatoonId;

		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 13;

		public LeavePlatoonRequest()
			: base("strRobocloudError", "strLeavePlatError", 0)
		{
			_oldPlatoonId = CacheDTO.platoon.platoonId;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			if (CacheDTO.platoon.isInPlatoon && CacheDTO.platoon.platoonId == _oldPlatoonId)
			{
				CacheDTO.platoon = new Platoon();
				PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
			}
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
			isDone = true;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}
	}
}
