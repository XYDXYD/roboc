using Network;
using Utility;

internal class NetworkConnectionEventHandlerLiteNetLibClientAlreadyConnected
{
	private IConnectionStatusHandlerClient _connectionStatusHandler;

	public NetworkConnectionEventHandlerLiteNetLibClientAlreadyConnected(NetworkClient networkClient, IConnectionStatusHandlerClient connectionStatusHandler)
	{
		_connectionStatusHandler = connectionStatusHandler;
		networkClient.RegisterHandler(33, OnClientDisconnect);
		networkClient.RegisterHandler(34, OnClientError);
	}

	private void OnClientDisconnect(Network.NetworkMessage netMsg)
	{
		_connectionStatusHandler.OnDisconnected();
	}

	private void OnClientError(Network.NetworkMessage netMsg)
	{
		ErrorMessage errorMessage = netMsg.ReadMessage<ErrorMessage>();
		NetworkError errorCode = (NetworkError)errorMessage.errorCode;
		Console.Log(errorCode.ToString());
	}
}
