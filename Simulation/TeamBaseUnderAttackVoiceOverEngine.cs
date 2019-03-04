using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeamBaseUnderAttackVoiceOverEngine : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private const float TIME_WITHOUT_REPLAY = 10f;

		private float _lastVOTime;

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer teamsContainer
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

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			destructionReporter.OnProtoniumDamageApplied += PlayTeamBaseCubesDamagedVO;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnProtoniumDamageApplied -= PlayTeamBaseCubesDamagedVO;
		}

		private void PlayTeamBaseCubesDamagedVO(DestructionData data)
		{
			if (data.targetType == TargetType.TeamBase && teamsContainer.IsMyTeam(data.hitPlayerId))
			{
				PlayEvent(AudioFabricGameEvents.VO_RBA_PR_UAttack);
			}
		}

		private void PlayEvent(AudioFabricGameEvents audioEvent)
		{
			float num = Time.get_time() - _lastVOTime;
			if (num > 10f)
			{
				_lastVOTime = Time.get_time();
				commandFactory.Build<PlayVOCommand>().Inject(AudioFabricEvent.StringEvents[(int)audioEvent]).Execute();
			}
		}
	}
}
