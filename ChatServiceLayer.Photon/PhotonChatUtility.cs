namespace ChatServiceLayer.Photon
{
	internal class PhotonChatUtility
	{
		private static PhotonChatUtility instance;

		private readonly PhotonClient _photonClient;

		public static PhotonChatUtility Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PhotonChatUtility();
				}
				return instance;
			}
		}

		private PhotonChatUtility()
		{
			_photonClient = new ChatClient();
		}

		public void GetRequestConnectionAndExecuteRequest(PhotonRequestContainer container)
		{
			_photonClient.CreateRequestWrapperAndExecuteRequest(container);
		}

		public ChatClient GetClient()
		{
			return _photonClient as ChatClient;
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
