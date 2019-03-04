internal interface IPhotonClient
{
	bool IsConnectedAndReady
	{
		get;
	}

	void SetEventRegistry(PhotonEventRegistry eventRegistry);
}
