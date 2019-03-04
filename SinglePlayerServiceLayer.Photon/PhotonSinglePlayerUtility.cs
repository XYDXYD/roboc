namespace SinglePlayerServiceLayer.Photon
{
	internal sealed class PhotonSinglePlayerUtility
	{
		private static PhotonSinglePlayerUtility instance;

		private readonly PhotonClient _photonClient;

		public static PhotonSinglePlayerUtility Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PhotonSinglePlayerUtility();
				}
				return instance;
			}
		}

		private PhotonSinglePlayerUtility()
		{
			_photonClient = new SinglePlayerClient();
		}

		public void Disconnect()
		{
			_photonClient.ForceDisconnect();
		}

		public void QueryWebServicesService(PhotonRequestContainer container)
		{
			_photonClient.CreateRequestWrapperAndExecuteRequest(container);
		}

		public IPhotonClient GetClientMustBeUsedOnlyForTheServiceContainer()
		{
			return _photonClient;
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
