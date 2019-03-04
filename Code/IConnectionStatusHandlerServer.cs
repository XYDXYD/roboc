internal interface IConnectionStatusHandlerServer
{
	void OnPlayerConnected(int player);

	void OnPlayerDisconnected(int player);
}
