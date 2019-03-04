using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ValidateSeasonRewardsRequest : SocialRequest, IValidateSeasonRewardsRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 52;

		public ValidateSeasonRewardsRequest()
			: base("strValidateSeasonRewards", "strValidateSeasonRewardsError", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>
			{
				{
					1,
					_userName
				}
			};
			val.OperationCode = OperationCode;
			return val;
		}

		public void Inject(string dependency)
		{
			_userName = dependency;
		}
	}
}
