using RCNetwork.Events;
using Svelto.IoC;
using System.Collections;

internal sealed class SendDisconnectToServerOnWorldSwitch : IInitialize
{
	[Inject]
	internal IDispatchWorldSwitching worldSwitching
	{
		get;
		private set;
	}

	[Inject]
	internal INetworkEventManagerClient networkEventManagerClient
	{
		private get;
		set;
	}

	void IInitialize.OnDependenciesInjected()
	{
		worldSwitching.OnWorldIsSwitching.Add(OnWorldSwitching());
	}

	private IEnumerator OnWorldSwitching()
	{
		networkEventManagerClient.SendEventToServer(NetworkEvent.ClientUnregistered, new NetworkDependency());
		yield break;
	}
}
