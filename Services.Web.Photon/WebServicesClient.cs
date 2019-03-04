using Authentication;
using ExitGames.Client.Photon;
using System;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class WebServicesClient : PhotonClient
	{
		protected override string serverAddressS3Key => "PhotonServicesServer";

		protected override string serverAddressLocalOverrideKey => "WebServicesServerAddress";

		protected override string applicationID => "WebServicesServer";

		protected override string appVersion => "v1";

		protected override int serviceConnectionInterval => 50;

		protected override int pingInterval => 3000;

		protected override int connectAttempts => 3;

		protected override float connectAttemptDelay => 1f;

		protected override byte duplicateLoginCode => 0;

		protected override byte ccuExceededCode => 13;

		protected override byte ccuCheckPassedCode => 14;

		protected override bool CCUCheckRequired => true;

		private byte _newSessionIdCode => 2;

		private byte _sessionIdParameterCode => 3;

		protected override byte maxPlayerPerRoom => 50;

		protected override int emptyRoomTtl => 10000;

		public WebServicesClient()
			: base(1)
		{
		}

		public override void OnEvent(EventData eventData)
		{
			if (eventData.Code == _newSessionIdCode)
			{
				User.SessionId = (string)eventData.Parameters[_sessionIdParameterCode];
			}
			else
			{
				base.OnEvent(eventData);
			}
		}

		public override void OnOperationResponse(OperationResponse operationResponse)
		{
			if (operationResponse.ReturnCode == 10)
			{
				DisconnectWithError(new Exception("Wrong number of authentication params sent to webservices server. Please check your game version"));
			}
			else if (operationResponse.ReturnCode == 11)
			{
				DisconnectWithError(new UserSuspendedException(StringTableBase<StringTable>.Instance.GetString("strSuspensionDetails")));
			}
			else
			{
				base.OnOperationResponse(operationResponse);
			}
		}

		protected override string GetAuthenticationParameters()
		{
			return base.GetAuthenticationParameters() + "," + User.SessionId;
		}

		protected override void OnConnectedToMaster()
		{
			Console.Log("Photon web services peer connected to master server");
			base.OnConnectedToMaster();
		}
	}
}
