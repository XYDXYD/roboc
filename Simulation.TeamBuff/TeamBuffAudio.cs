using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace Simulation.TeamBuff
{
	internal class TeamBuffAudio : IInitialize, IWaitForFrameworkDestruction
	{
		private const int TIME_AFTER_GAMESTART_TO_PLAY_VO = 5;

		private int _buffedTeamId = -1;

		[Inject]
		internal BuffTeamObserver buffTeamObserver
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal VOManager voManager
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			buffTeamObserver.AddAction(new ObserverAction<TeamBuffDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameEndedObserver.OnGameEnded += DisableVO;
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			buffTeamObserver.RemoveAction(new ObserverAction<TeamBuffDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameEndedObserver.OnGameEnded -= DisableVO;
		}

		private void PlayTeamBuffVO(ref TeamBuffDependency dependency)
		{
			if (dependency.isStartOfGame)
			{
				TaskRunner.get_Instance().Run(PlayVOAfterSomeTime(dependency.teamBuffs, dependency.disconnectedTeamID));
			}
			else if (dependency.isNeutralised)
			{
				PlayVO(AudioFabricGameEvents.VO_RBA_TeamBuff_Removed);
				_buffedTeamId = -1;
			}
			else
			{
				PlayVO(dependency.teamBuffs, dependency.disconnectedTeamID);
			}
		}

		private IEnumerator PlayVOAfterSomeTime(float[] teamBuffs, int disconnectedTeamID)
		{
			yield return (object)new WaitForSecondsEnumerator(5f);
			PlayVO(teamBuffs, disconnectedTeamID);
		}

		private void PlayVO(float[] teamBuff, int disconnectedTeamID)
		{
			int myTeam = playerTeamsContainer.GetMyTeam();
			int num = (myTeam == 0) ? 1 : 0;
			float num2 = teamBuff[myTeam];
			float num3 = teamBuff[num];
			int num4 = myTeam;
			if (num3 > 0f)
			{
				num4 = num;
			}
			bool flag = num4 != _buffedTeamId;
			_buffedTeamId = num4;
			if (flag)
			{
				if (num2 > num3)
				{
					PlayVO(AudioFabricGameEvents.VO_RBA_TeamDisconnected_TeamBuff);
				}
				else
				{
					PlayVO(AudioFabricGameEvents.VO_RBA_EnemyDisconnected_EnemyTeamBuff);
				}
			}
			else if (disconnectedTeamID == myTeam)
			{
				PlayVO(AudioFabricGameEvents.VO_RBA_TeamDisconnected);
			}
			else
			{
				PlayVO(AudioFabricGameEvents.VO_RBA_EnemyDisconnected);
			}
		}

		private void PlayVO(AudioFabricGameEvents eventName)
		{
			voManager.PlayVO(AudioFabricEvent.Name(eventName));
		}

		private unsafe void DisableVO(bool won)
		{
			buffTeamObserver.RemoveAction(new ObserverAction<TeamBuffDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
	}
}
