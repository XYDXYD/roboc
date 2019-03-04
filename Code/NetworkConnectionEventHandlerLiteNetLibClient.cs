using Network;
using Utility;

internal class NetworkConnectionEventHandlerLiteNetLibClient
{
	private IConnectionStatusHandlerClient _connectionStatusHandler;

	private float _maxDelay = 1f;

	private bool _connected;

	public NetworkConnectionEventHandlerLiteNetLibClient(NetworkClient networkClient, IConnectionStatusHandlerClient connectionStatusHandler)
	{
		_connectionStatusHandler = connectionStatusHandler;
		networkClient.RegisterHandler(32, OnClientConnect);
		networkClient.RegisterHandler(33, OnClientDisconnect);
		networkClient.RegisterHandler(34, OnClientError);
	}

	public void SetMaxDelay(float value)
	{
		_maxDelay = value;
	}

	private void OnClientConnect(Network.NetworkMessage netMsg)
	{
		_connected = true;
		_connectionStatusHandler.OnConnectionSucceeded();
		netMsg.conn.SetMaxDelay(_maxDelay / 1000f);
	}

	private void OnClientDisconnect(Network.NetworkMessage netMsg)
	{
		if (_connected)
		{
			_connectionStatusHandler.OnDisconnected();
		}
		else
		{
			_connectionStatusHandler.OnConnectionFailed();
		}
		_connected = false;
	}

	private void OnClientError(Network.NetworkMessage netMsg)
	{
		ErrorMessage errorMessage = netMsg.ReadMessage<ErrorMessage>();
		NetworkError errorCode = (NetworkError)errorMessage.errorCode;
		Console.LogError(errorCode.ToString());
		if (IsConnectionFailure(errorCode))
		{
			_connectionStatusHandler.OnConnectionFailed(errorCode.ToString());
		}
	}

	private static bool IsConnectionFailure(NetworkError err)
	{
		return err == NetworkError.VersionMismatch || err == NetworkError.CRCMismatch || err == NetworkError.DNSFailure || err == NetworkError.VersionMismatch;
	}
}
