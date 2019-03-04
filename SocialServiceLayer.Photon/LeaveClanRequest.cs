using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class LeaveClanRequest : SocialRequest, ILeaveClanRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 40;

		public LeaveClanRequest()
			: base("strLeaveClanErrorTitle", "strLeaveClanErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		public override void OnOpResponse(OperationResponse response)
		{
			CacheDTO.MyClanInfo = null;
			CacheDTO.MyClanMembers = new Dictionary<string, ClanMember>();
			base.OnOpResponse(response);
		}
	}
}
