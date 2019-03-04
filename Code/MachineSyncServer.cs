using Svelto.IoC;

internal sealed class MachineSyncServer
{
	private IMachineUpdater _machineUpdater;

	[Inject]
	public NetworkClientPool playerManager
	{
		private get;
		set;
	}

	[Inject]
	public NetworkMachineManager machineManager
	{
		private get;
		set;
	}

	[Inject]
	public MachineTimeManager machineTimeManager
	{
		private get;
		set;
	}

	public byte[] GetByteArray()
	{
		return null;
	}
}
