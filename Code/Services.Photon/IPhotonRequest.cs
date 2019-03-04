using ExitGames.Client.Photon;
using System;

namespace Services.Photon
{
	internal interface IPhotonRequest
	{
		bool isEncrypted
		{
			get;
		}

		void OnOpResponse(OperationResponse response);

		OperationRequest GetOperationRequest();

		void OnSendOperationFailed(Exception obj);

		void OnClientDisconnected(bool isUnexpected, Exception managedException);
	}
}
