using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetRobotBayCustomisationsRequest : WebServicesCachedRequest<GetRobotBayCustomisationsResponse>, IGetRobotBayCustomisationsRequest, IServiceRequest<string>, IAnswerOnComplete<GetRobotBayCustomisationsResponse>, IServiceRequest
	{
		private string _robotUniqueID;

		protected override byte OperationCode => 218;

		public GetRobotBayCustomisationsRequest()
			: base("strRobocloudError", "strGetRobotCustomisationsRequestError", 0)
		{
		}

		public void Inject(string robotUniqueID)
		{
			_robotUniqueID = robotUniqueID;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[54] = _robotUniqueID;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override GetRobotBayCustomisationsResponse ProcessResponse(OperationResponse response)
		{
			string baySkinId_ = (string)response.Parameters[234];
			string spawnEffectId_ = (string)response.Parameters[235];
			string deathEffectId_ = (string)response.Parameters[236];
			return new GetRobotBayCustomisationsResponse(spawnEffectId_, deathEffectId_, baySkinId_);
		}

		void IGetRobotBayCustomisationsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
