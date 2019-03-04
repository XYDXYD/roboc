using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class ValidateCurrentMachineRequest : WebServicesRequest<ValidateCurrentMachineResult>, IValidateCurrentMachineRequest, IServiceRequest, IAnswerOnComplete<ValidateCurrentMachineResult>
	{
		private LobbyType _gameMode;

		protected override byte OperationCode => 102;

		public ValidateCurrentMachineRequest()
			: base("strRobotValidation", "strRobotValidationError", 0)
		{
		}

		public void Inject(LobbyType gameMode)
		{
			_gameMode = gameMode;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters.Add(134, _gameMode);
			val.OperationCode = OperationCode;
			return val;
		}

		protected override ValidateCurrentMachineResult ProcessResponse(OperationResponse response)
		{
			return (ValidateCurrentMachineResult)response.Parameters[111];
		}
	}
}
