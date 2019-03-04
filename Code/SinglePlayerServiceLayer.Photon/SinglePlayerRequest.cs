using Services;
using Services.Photon;

namespace SinglePlayerServiceLayer.Photon
{
	internal abstract class SinglePlayerRequest<TData> : PhotonRequest<TData>
	{
		protected override short UnexpectedErrorCode => 2;

		protected override string ServerName => ServicesServerNames.SinglePlayerServices;

		protected override byte GuidParameterCode => 0;

		public SinglePlayerRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
		}

		public override void Execute()
		{
			PhotonSinglePlayerUtility.Instance.QueryWebServicesService(new PhotonRequestContainer(this));
		}

		protected void Disconnect()
		{
			PhotonSinglePlayerUtility.Instance.Disconnect();
		}
	}
	internal abstract class SinglePlayerRequest : PhotonRequest
	{
		protected override short UnexpectedErrorCode => 2;

		protected override string ServerName => ServicesServerNames.SinglePlayerServices;

		protected override byte GuidParameterCode => 0;

		public SinglePlayerRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
		}

		public override void Execute()
		{
			PhotonSinglePlayerUtility.Instance.QueryWebServicesService(new PhotonRequestContainer(this));
		}

		protected void Disconnect()
		{
			PhotonSinglePlayerUtility.Instance.Disconnect();
		}
	}
}
