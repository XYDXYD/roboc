using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal class ValidateUserRequest : WebServicesRequest<ValidateUserRequestData>, IValidateUserRequest, IServiceRequest, IAnswerOnComplete<ValidateUserRequestData>
	{
		protected override byte OperationCode => 105;

		public ValidateUserRequest()
			: base("strRobocloudError", "strUnableValidateUserRequest", 0)
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

		protected override ValidateUserRequestData ProcessResponse(OperationResponse response)
		{
			ValidateUserRequestData validateUserRequestData = new ValidateUserRequestData();
			validateUserRequestData.removedObsoleteCubes = (bool)response.Parameters[113];
			validateUserRequestData.removedNotOwnedCubes = (bool)response.Parameters[114];
			validateUserRequestData.specialRewardTitle = response.Parameters[115].ToString();
			validateUserRequestData.specialRewardBody = response.Parameters[116].ToString();
			validateUserRequestData.refundedObsoleteCubes = (bool)response.Parameters[117];
			validateUserRequestData.cubesHaveBeenReplaced = (bool)response.Parameters[118];
			validateUserRequestData.isNewUser = (bool)response.Parameters[119];
			validateUserRequestData.cratesRefundedTPAmount = (int)response.Parameters[212];
			validateUserRequestData.abTest = ((!response.Parameters.ContainsKey(166)) ? null : ((string)response.Parameters[166]));
			validateUserRequestData.abTestGroup = ((!response.Parameters.ContainsKey(167)) ? null : ((string)response.Parameters[167]));
			return validateUserRequestData;
		}
	}
}
