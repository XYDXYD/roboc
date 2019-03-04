using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class FusionShieldActivationAudioPlayer : IInitialize, IWaitForFrameworkDestruction
	{
		private string _teamLowPowerVO = AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_BS_Down);

		private string _enemyLowPowerVO = AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_BS_E_Down);

		private string _teamFullPowerVO = AudioFabricEvent.Name(AudioFabricGameEvents.FusionShieldOnVO);

		private string _fullPowerSound = AudioFabricEvent.Name(AudioFabricGameEvents.FusionShieldOn);

		private string _lowPowerSound = AudioFabricEvent.Name(AudioFabricGameEvents.FusionShieldOff);

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		public FusionShieldsObserver shieldsObserver
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			shieldsObserver.RegisterShieldStateChanged(PlaySound);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			shieldsObserver.UnregisterShieldStateChanged(PlaySound);
		}

		private void PlaySound(int teamId, bool fullPower)
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.TeamBase, teamId);
			GameObject machineRoot = machineRootContainer.GetMachineRoot(TargetType.TeamBase, activeMachine);
			EventManager.get_Instance().PostEvent((!fullPower) ? _lowPowerSound : _fullPowerSound, 0, (object)null, machineRoot);
			if (playerTeamsContainer.IsMyTeam(teamId))
			{
				if (fullPower)
				{
					commandFactory.Build<PlayVOCommand>().Inject(_teamFullPowerVO).Execute();
				}
				else
				{
					commandFactory.Build<PlayVOCommand>().Inject(_teamLowPowerVO).Execute();
				}
			}
			else if (!fullPower)
			{
				commandFactory.Build<PlayVOCommand>().Inject(_enemyLowPowerVO).Execute();
			}
		}
	}
}
