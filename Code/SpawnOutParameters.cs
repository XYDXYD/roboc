internal sealed class SpawnOutParameters
{
	public int playerId;

	public MachineRootContainer machineRootContainer;

	public NetworkMachineManager machineManager;

	public LivePlayersContainer livePlayersController;

	public SpawnOutParameters(int id, MachineRootContainer root, NetworkMachineManager machine, LivePlayersContainer live)
	{
		playerId = id;
		machineRootContainer = root;
		machineManager = machine;
		livePlayersController = live;
	}
}
