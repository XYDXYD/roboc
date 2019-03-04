namespace LobbyServiceLayer.Photon
{
	internal class PhotonLobbyUtility
	{
		private static PhotonLobbyUtility instance;

		private PhotonClient _photonClient;

		public static PhotonLobbyUtility Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PhotonLobbyUtility();
				}
				return instance;
			}
		}

		private PhotonLobbyUtility()
		{
			_photonClient = new LobbyClient();
		}

		public void GetRequestConnection(PhotonRequestContainer container)
		{
			if (_photonClient == null)
			{
				_photonClient = new LobbyClient();
			}
			_photonClient.CreateRequestWrapperAndExecuteRequest(container);
		}

		public IPhotonClient GetClientMustBeUsedOnlyForTheServiceContainer()
		{
			return _photonClient;
		}

		public void PlatoonUpdate(Platoon platoon)
		{
			(_photonClient as LobbyClient).PlatoonUpdate(platoon);
		}

		public void Disconnect()
		{
			_photonClient.ForceDisconnect();
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
