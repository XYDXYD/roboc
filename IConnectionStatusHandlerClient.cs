internal interface IConnectionStatusHandlerClient
{
	void OnConnectionFailed(string error = null);

	void OnConnectionLost();

	void OnDisconnected();

	void OnConnectionSucceeded();
}
