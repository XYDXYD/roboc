using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal sealed class CapturePointAudioEngine : IQueryingEntityViewEngine, IInitialize, IEngine
	{
		private const float TIME_WITHOUT_REPLAY = 10f;

		private PlayVOCommand _voCommand;

		private Dictionary<AudioFabricGameEvents, float> _lastVOTime = new Dictionary<AudioFabricGameEvents, float>();

		[Inject]
		internal PlayerTeamsContainer teamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe CapturePointAudioEngine(CapturePointNotificationObserver observer)
		{
			observer.AddAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			for (int i = 0; i < 3; i++)
			{
				_lastVOTime[GetVOEvent((CapturePointVisualPosition)i)] = 0f;
			}
		}

		public void OnDependenciesInjected()
		{
			_voCommand = commandFactory.Build<PlayVOCommand>();
		}

		private void HandleOnNotification(ref CapturePointNotificationDependency parameter)
		{
			switch (parameter.notification)
			{
			case CapturePointNotification.CaptureStoppedNoAttackers:
			case CapturePointNotification.CaptureStoppedByDefenders:
				break;
			case CapturePointNotification.CaptureLocked:
			case CapturePointNotification.CaptureUnlocked:
			case CapturePointNotification.SegmentCompleted:
				break;
			case CapturePointNotification.CaptureStarted:
				PlayAudioOnCaptureStarted(parameter);
				break;
			case CapturePointNotification.CaptureCompleted:
				PlayAudioOnCaptureCompleted(parameter);
				break;
			case CapturePointNotification.Dominating:
				TryPlayDominatingVO(parameter.attackingTeam);
				break;
			}
		}

		private void TryPlayDominatingVO(int dominatingTeam)
		{
			if (teamsContainer.IsMyTeam(dominatingTeam))
			{
				AudioFabricGameEvents audioFabricGameEvents = AudioFabricGameEvents.VO_RBA_Dominating;
				_voCommand.Inject(AudioFabricEvent.StringEvents[(int)audioFabricGameEvents]).Execute();
				EventManager.get_Instance().PostEvent("Pit_Legendary");
			}
		}

		private void PlayAudioOnCaptureStarted(CapturePointNotificationDependency parameter)
		{
			AudioNode audioNode = entityViewsDB.QueryEntityView<AudioNode>(parameter.id);
			if (teamsContainer.IsMyTeam(parameter.defendingTeam))
			{
				CapturePointVisualPosition visualPositionId = audioNode.visualPositionComponent.visualPositionId;
				PlayCaptureStartedVO(visualPositionId);
			}
			if (!teamsContainer.IsMyTeam(parameter.attackingTeam))
			{
				EventManager.get_Instance().PostEvent(audioNode.audioComponent.loopAudioEvent, 0);
			}
		}

		private void PlayCaptureStartedVO(CapturePointVisualPosition visualId)
		{
			AudioFabricGameEvents vOEvent = GetVOEvent(visualId);
			float num = Time.get_time() - _lastVOTime[vOEvent];
			if (num > 10f)
			{
				_lastVOTime[vOEvent] = Time.get_time();
				_voCommand.Inject(AudioFabricEvent.StringEvents[(int)vOEvent]).Execute();
			}
		}

		private AudioFabricGameEvents GetVOEvent(CapturePointVisualPosition towerPosition)
		{
			AudioFabricGameEvents result = AudioFabricGameEvents.VO_RBA_NT_UAttack;
			switch (towerPosition)
			{
			case CapturePointVisualPosition.Near:
				result = AudioFabricGameEvents.VO_RBA_NT_UAttack;
				break;
			case CapturePointVisualPosition.Middle:
				result = AudioFabricGameEvents.VO_RBA_MT_UAttack;
				break;
			case CapturePointVisualPosition.Far:
				result = AudioFabricGameEvents.VO_RBA_FT_UAttack;
				break;
			}
			return result;
		}

		private void PlayAudioOnCaptureCompleted(CapturePointNotificationDependency parameter)
		{
			AudioNode audioNode = entityViewsDB.QueryEntityView<AudioNode>(parameter.id);
			bool flag = teamsContainer.IsMyTeam(parameter.attackingTeam);
			CapturePointVisualPosition visualPositionId = audioNode.visualPositionComponent.visualPositionId;
			PlayCaptureCompletedVO(visualPositionId, flag);
			if (flag)
			{
				int playersCountOnTeam = teamsContainer.GetPlayersCountOnTeam(TargetType.CapturePoint, parameter.attackingTeam);
				PlayAudioEffect(playersCountOnTeam, capturedByMyTeam: true);
			}
			else if (parameter.defendingTeam != -1)
			{
				int count = teamsContainer.GetPlayersCountOnTeam(TargetType.CapturePoint, parameter.defendingTeam) + 1;
				PlayAudioEffect(count, capturedByMyTeam: false);
			}
			else
			{
				PlayAudioEffect(1, capturedByMyTeam: true);
			}
		}

		private void PlayAudioEffect(int count, bool capturedByMyTeam)
		{
			switch (count)
			{
			case 1:
			{
				int num = (!capturedByMyTeam) ? 51 : 46;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[num], 0);
				break;
			}
			case 2:
			{
				int num = (!capturedByMyTeam) ? 53 : 50;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[num], 0);
				break;
			}
			case 3:
			{
				int num = (!capturedByMyTeam) ? 46 : 51;
				EventManager.get_Instance().PostEvent(AudioFabricEvent.StringEvents[num], 0);
				break;
			}
			}
		}

		private void PlayCaptureCompletedVO(CapturePointVisualPosition visualId, bool capturedByMyTeam)
		{
			AudioFabricGameEvents audioFabricGameEvents = AudioFabricGameEvents.VO_RBA_NT_Captured;
			switch (visualId)
			{
			case CapturePointVisualPosition.Near:
				audioFabricGameEvents = ((!capturedByMyTeam) ? AudioFabricGameEvents.VO_RBA_NT_E_Captured : AudioFabricGameEvents.VO_RBA_NT_Captured);
				break;
			case CapturePointVisualPosition.Middle:
				audioFabricGameEvents = ((!capturedByMyTeam) ? AudioFabricGameEvents.VO_RBA_MT_E_Captured : AudioFabricGameEvents.VO_RBA_MT_Captured);
				break;
			case CapturePointVisualPosition.Far:
				audioFabricGameEvents = ((!capturedByMyTeam) ? AudioFabricGameEvents.VO_RBA_FT_E_Captured : AudioFabricGameEvents.VO_RBA_FT_Captured);
				break;
			}
			_voCommand.Inject(AudioFabricEvent.StringEvents[(int)audioFabricGameEvents]).Execute();
		}

		public void Ready()
		{
		}
	}
}
