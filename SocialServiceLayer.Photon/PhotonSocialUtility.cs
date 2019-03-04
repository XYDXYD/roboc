using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class PhotonSocialUtility
	{
		private static PhotonSocialUtility instance;

		private SocialClient _photonClient;

		public static PhotonSocialUtility Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PhotonSocialUtility();
				}
				return instance;
			}
		}

		private PhotonSocialUtility()
		{
			_photonClient = new SocialClient();
		}

		public void GetRequestConnectionAndExecuteRequest(PhotonRequestContainer container)
		{
			_photonClient.CreateRequestWrapperAndExecuteRequest(container);
		}

		public IPhotonClient GetClientMustBeUsedOnlyForTheServiceContainer()
		{
			return _photonClient;
		}

		public void RaiseInternalEvent<TEventListener, TEventData>(TEventData data) where TEventListener : IServiceEventListener<TEventData>
		{
			_photonClient.RaiseInternalEvent<TEventListener, TEventData>(data);
		}

		internal static void TearDown()
		{
			if (instance != null)
			{
				instance._photonClient.TearDown();
			}
		}
	}
}
