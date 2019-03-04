using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class PhotonWebServicesUtility
	{
		private static PhotonWebServicesUtility instance;

		private readonly PhotonClient _photonClient;

		public static PhotonWebServicesUtility Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PhotonWebServicesUtility();
				}
				return instance;
			}
		}

		private PhotonWebServicesUtility()
		{
			_photonClient = new WebServicesClient();
		}

		public void AddCCUExceededEventHandler(Action<int, List<string>> OnCCuExceeded)
		{
			_photonClient.AddCCUExceededEventHandler(OnCCuExceeded);
		}

		public void AddCCUPassedEventHandler(Action OnCCUPassed)
		{
			_photonClient.AddCCUPassedEventHandler(OnCCUPassed);
		}

		public void RemoveCCUExceededEventHandler(Action<int, List<string>> OnCCuExceeded)
		{
			_photonClient.RemoveCCUExceededEventHandler(OnCCuExceeded);
		}

		public void RemoveCCUPassedEventHandler(Action OnCCUPassed)
		{
			_photonClient.RemoveCCUPassedEventHandler(OnCCUPassed);
		}

		public void QueryWebServicesService(PhotonRequestContainer request)
		{
			_photonClient.CreateRequestWrapperAndExecuteRequest(request);
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
