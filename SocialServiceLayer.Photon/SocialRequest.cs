using Services;
using Services.Photon;

namespace SocialServiceLayer.Photon
{
	internal abstract class SocialRequest : PhotonRequest
	{
		protected override short UnexpectedErrorCode => 1;

		protected override string ServerName => ServicesServerNames.SocialServices;

		protected override byte GuidParameterCode => 0;

		public SocialRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public override void Execute()
		{
			PhotonSocialUtility.Instance.GetRequestConnectionAndExecuteRequest(new PhotonRequestContainer(this));
		}
	}
	internal abstract class SocialRequest<TData> : PhotonRequest<TData>
	{
		protected override short UnexpectedErrorCode => 1;

		protected override string ServerName => ServicesServerNames.SocialServices;

		protected override byte GuidParameterCode => 0;

		public SocialRequest(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public override void Execute()
		{
			PhotonSocialUtility.Instance.GetRequestConnectionAndExecuteRequest(new PhotonRequestContainer(this));
		}
	}
}
