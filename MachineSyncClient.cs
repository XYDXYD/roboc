using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.IO;

internal sealed class MachineSyncClient
{
	[Inject]
	public PlayerTeamsContainer playerTeamsContainer
	{
		private get;
		set;
	}

	[Inject]
	public PlayerMachinesContainer playerMachinesContainer
	{
		private get;
		set;
	}

	[Inject]
	public LivePlayersContainer livePlayersContainer
	{
		private get;
		set;
	}

	[Inject]
	public RemoteClientHistoryClient remoteClientHistory
	{
		private get;
		set;
	}

	public void ReceiveIndividualMachineDataFromServer(byte[] data)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				MachineMotionDependency machineMotionDependency = new MachineMotionDependency(binaryReader.ReadBytes(MachineMotionDependency.GetSize()));
				WeaponRaycastInfo weaponRaycastInfo = new WeaponRaycastInfo();
				weaponRaycastInfo.aimPoint = machineMotionDependency.targetPoint;
				if (!playerTeamsContainer.IsMe(TargetType.Player, machineMotionDependency.playerId) && livePlayersContainer.IsPlayerAlive(TargetType.Player, machineMotionDependency.playerId))
				{
					PlayerMachineMotionHistoryFrame.RigidBodyState rigidBodyState = new PlayerMachineMotionHistoryFrame.RigidBodyState();
					rigidBodyState.worldCOM = machineMotionDependency.rbState.worldCOM;
					rigidBodyState.rotation = machineMotionDependency.rbState.rotation;
					rigidBodyState.angularVelocity = machineMotionDependency.rbState.angularVelocity;
					PlayerMachineMotionHistoryFrame playerMachineMotionHistoryFrame = new PlayerMachineMotionHistoryFrame(rigidBodyState, weaponRaycastInfo);
					playerMachineMotionHistoryFrame.timeStamp = machineMotionDependency.timeStamp;
					int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, machineMotionDependency.playerId);
					PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame> machineHistory = remoteClientHistory.GetMachineHistory(activeMachine);
					int stateCount = machineHistory.GetStateCount();
					int index = (machineHistory.GetWritingState() - 1 + stateCount) % stateCount;
					PlayerMachineMotionHistoryFrame state = machineHistory.GetState(index);
					if (machineMotionDependency.timeStamp > state.timeStamp || state.timeStamp < 0f)
					{
						machineHistory.AddData(playerMachineMotionHistoryFrame);
					}
				}
			}
		}
	}
}
