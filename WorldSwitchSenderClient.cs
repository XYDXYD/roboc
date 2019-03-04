using RCNetwork.Events;
using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.IoC;

internal sealed class WorldSwitchSenderClient : IHandleWorldSwitchInput, IInputComponent, IComponent
{
	[Inject]
	public INetworkEventManagerClient networkEventManager
	{
		private get;
		set;
	}

	[Inject]
	public WeakReference<IEntitySystemContext> root
	{
		private get;
		set;
	}

	public void HandleWorldSwitchInput(bool buttonDown)
	{
		if (buttonDown)
		{
			networkEventManager.SendEventToServer(NetworkEvent.ClientUnregistered, new NetworkDependency());
			root.get_Target().RemoveComponent(this);
		}
	}
}
