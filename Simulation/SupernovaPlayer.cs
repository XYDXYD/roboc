using Simulation.BattleArena;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class SupernovaPlayer
	{
		private SupernovaEffect _effect;

		private Transform _transform;

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal SupernovaAudioObserver supernovaAudioObserver
		{
			private get;
			set;
		}

		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		public void PlaySupernova(int winningTeam, Action onComplete)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			int num = (winningTeam == 0) ? 1 : 0;
			_effect.get_gameObject().SetActive(true);
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.TeamBase, num);
			Transform transform = machineRootContainer.GetMachineRoot(TargetType.TeamBase, activeMachine).get_transform();
			FusionShieldController componentInChildren = transform.GetComponentInChildren<FusionShieldController>();
			componentInChildren.get_gameObject().SetActive(false);
			_transform.set_position(componentInChildren.get_transform().get_position());
			_effect.StartEffect(num, playerTeamsContainer.IsMyTeam(num), transform, onComplete);
			supernovaAudioObserver.PlaySupernovaAudio(_effect.get_gameObject());
		}

		internal void Register(SupernovaEffect supernovaEffect)
		{
			_effect = supernovaEffect;
			_transform = supernovaEffect.get_transform();
		}
	}
}
