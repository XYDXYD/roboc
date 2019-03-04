using ExitGames.Client.Photon;
using Services;
using Services.Photon;

namespace LobbyServiceLayer.Photon
{
	internal abstract class LobbyRequest : PhotonRequest
	{
		protected override short UnexpectedErrorCode => -1;

		protected override string ServerName => ServicesServerNames.LobbyServices;

		protected override byte GuidParameterCode => 0;

		public LobbyRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public override void Execute()
		{
			PhotonLobbyUtility.Instance.GetRequestConnection(new PhotonRequestContainer(this));
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
	internal abstract class LobbyRequest<TData> : PhotonRequest<TData>
	{
		protected override short UnexpectedErrorCode => -1;

		protected override string ServerName => ServicesServerNames.LobbyServices;

		protected override byte GuidParameterCode => 0;

		public LobbyRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public override void Execute()
		{
			PhotonLobbyUtility.Instance.GetRequestConnection(new PhotonRequestContainer(this));
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
