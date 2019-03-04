using RCNetwork.Events;
using RCNetwork.UNet.Client;
using Svelto.Command.Dispatcher;
using Utility;

internal sealed class NetworkConnectionStatusHandlerClient : IConnectionStatusHandlerClient
{
	private IEventDispatcher _eventDispatcher;

	private string _hostIP;

	private int _hostPort;

	public NetworkConnectionStatusHandlerClient(IEventDispatcher dispatcher)
	{
		_eventDispatcher = dispatcher;
	}

	public void SetHostParameters(string ip, int port)
	{
		_hostIP = ip;
		_hostPort = port;
	}

	public void OnConnectionSucceeded()
	{
		Console.Log("connected to game server :" + _hostIP + " " + _hostPort);
		_eventDispatcher.Dispatch<NetworkEvent>(NetworkEvent.OnConnectedToServer);
	}

	public void OnConnectionFailed(string error)
	{
		Console.Log("failed to connect to game server, error: " + error + " " + _hostIP + " " + _hostPort);
		_eventDispatcher.Dispatch<NetworkEvent>(NetworkEvent.OnFailedToConnectToServer, new object[1]
		{
			error
		});
	}

	public void OnConnectionLost()
	{
	}

	public void OnDisconnected()
	{
		Console.Log("disconnected from game server " + _hostIP + " " + _hostPort);
		EventRouterClientLiteNetLib.ClearBufferOnDisconnect();
		_eventDispatcher.Dispatch<NetworkEvent>(NetworkEvent.OnDisconnectedFromServer);
	}
}
