using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal class NetworkInputRecievedManager
	{
		private Dictionary<int, MachineInputWrapperRemoteClient> _inputWrappers = new Dictionary<int, MachineInputWrapperRemoteClient>();

		[Inject]
		public RemoteClientHistoryClient remoteClientHistory
		{
			protected get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			protected get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			protected get;
			set;
		}

		public void RegisterUsersInputWrapper(int player, MachineInputWrapperRemoteClient inputWrapper)
		{
			_inputWrappers[player] = inputWrapper;
		}

		public void UnregisterUsersInputWrapper(int player)
		{
			_inputWrappers.Remove(player);
		}

		public void InputReceived(MultiPlayerInputChangedDependency dependency)
		{
			foreach (KeyValuePair<int, MultiPlayerInputChangedDependency.PlayerInput> item in dependency.singlePlayerInput)
			{
				if (!playerTeamsContainer.IsMe(TargetType.Player, item.Key) && _inputWrappers.ContainsKey(item.Key))
				{
					AppendInputStateToInputHistory(item.Key, item.Value);
					MachineInputWrapperRemoteClient machineInputWrapperRemoteClient = _inputWrappers[item.Key];
					machineInputWrapperRemoteClient.horizontalAxis = item.Value.horizontal;
					machineInputWrapperRemoteClient.forwardAxis = item.Value.vertical;
					machineInputWrapperRemoteClient.moveUpwards = item.Value.jump;
					machineInputWrapperRemoteClient.moveDown = item.Value.crouch;
					machineInputWrapperRemoteClient.pulseAR = item.Value.pulseAR;
					machineInputWrapperRemoteClient.toggleLight = item.Value.toggleLights;
					machineInputWrapperRemoteClient.strafeLeft = item.Value.strafeLeft;
					machineInputWrapperRemoteClient.strafeRight = item.Value.strafeRight;
				}
			}
		}

		private void AppendInputStateToInputHistory(int player, MultiPlayerInputChangedDependency.PlayerInput playerInput)
		{
			PlayerHistoryBuffer<PlayerInputHistoryFrame> inputHistory = remoteClientHistory.GetInputHistory(player);
			if (inputHistory != null && playerInput != null)
			{
				PlayerInputHistoryFrame playerInputHistoryFrame = new PlayerInputHistoryFrame();
				playerInputHistoryFrame.horizontalAxis = playerInput.horizontal;
				playerInputHistoryFrame.forwardAxis = playerInput.vertical;
				playerInputHistoryFrame.moveUpwards = playerInput.jump;
				playerInputHistoryFrame.moveDown = playerInput.crouch;
				playerInputHistoryFrame.pulseAR = playerInput.pulseAR;
				playerInputHistoryFrame.toggleLight = playerInput.toggleLights;
				playerInputHistoryFrame.strafeLeft = playerInput.strafeLeft;
				playerInputHistoryFrame.strafeRight = playerInput.strafeRight;
				inputHistory.AddData(playerInputHistoryFrame);
			}
		}
	}
}
