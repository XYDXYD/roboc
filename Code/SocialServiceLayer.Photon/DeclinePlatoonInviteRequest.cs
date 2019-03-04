using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;

namespace SocialServiceLayer.Photon
{
	internal class DeclinePlatoonInviteRequest : SocialRequest, IDeclinePlatoonInviteRequest, ITask, IServiceRequest, IAnswerOnComplete, IAbstractTask
	{
		protected override byte OperationCode => 12;

		public bool isDone
		{
			get;
			private set;
		}

		public DeclinePlatoonInviteRequest()
			: base("strRobocloudError", "strDeclinePlatInviteError", 0)
		{
		}

		public override void OnOpResponse(OperationResponse response)
		{
			CacheDTO.platoonInvite = null;
			isDone = true;
			base.OnOpResponse(response);
		}

		protected override void OnFailed(Exception e)
		{
			base.OnFailed(e);
			isDone = true;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}
	}
}
