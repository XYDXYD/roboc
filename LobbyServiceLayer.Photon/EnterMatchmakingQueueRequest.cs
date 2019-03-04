using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace LobbyServiceLayer.Photon
{
	internal class EnterMatchmakingQueueRequest : LobbyRequest<int>, IEnterMatchmakingQueueRequest, IServiceRequest<EnterMatchmakingQueueDependency>, IAnswerOnComplete<int>, IServiceRequest
	{
		private EnterMatchmakingQueueDependency _dependency;

		protected override byte OperationCode => 0;

		public EnterMatchmakingQueueRequest()
			: base("strLobbyError", "strLobbyJoinFail", 0)
		{
		}

		public void Inject(EnterMatchmakingQueueDependency dependency)
		{
			PhotonLobbyUtility.Instance.PlatoonUpdate(dependency.Platoon);
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			string value = string.Empty;
			if (_dependency.Platoon != null)
			{
				value = _dependency.Platoon.platoonId;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(24, _dependency.BrawlVersionNumber);
			dictionary.Add(2, value);
			dictionary.Add(3, _dependency.GarageSlot);
			dictionary.Add(4, _dependency.PlatoonSize);
			dictionary.Add(14, _dependency.IsPlatoonLeader);
			dictionary.Add(30, _dependency.LobbyGameMode);
			dictionary.Add(29, _dependency.GameModePreferences.Serialize());
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}

		protected override int ProcessResponse(OperationResponse response)
		{
			return (int)response.Parameters[13];
		}

		protected override void OnFailed(Exception exception)
		{
			_serviceBehaviour.reasonID = null;
			_serviceBehaviour.serverReason = ((LobbyReasonCode)_serviceBehaviour.errorCode).ToString();
			if (_serviceBehaviour.errorCode != 0)
			{
				base.OnFailed(exception);
			}
		}
	}
}
