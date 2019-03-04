using Battle;
using Simulation.BattleArena.CapturePoint;
using Simulation.BattleArena.Equalizer;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.BattleArena
{
	internal class BattleArenaHUDPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private EqualizerNotificationObserver _eqObserver;

		private CapturePointNotificationObserver _notificationObserver;

		private CapturePointProgressObserver _progressObserver;

		private FasterList<IBattleArenaHUDView> _views = new FasterList<IBattleArenaHUDView>();

		[Inject]
		internal BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal HealthTracker healthTraker
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

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
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

		public unsafe BattleArenaHUDPresenter(EqualizerNotificationObserver eqObserver, CapturePointNotificationObserver notificationObserver, CapturePointProgressObserver progressObserver)
		{
			_eqObserver = eqObserver;
			_notificationObserver = notificationObserver;
			_progressObserver = progressObserver;
			eqObserver.AddAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			notificationObserver.AddAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			progressObserver.AddAction(new ObserverAction<TeamBaseStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void OnDependenciesInjected()
		{
			healthTraker.OnEntityHealthChanged += HandleOnEntityHealthChanged;
			spawnDispatcher.OnEntitySpawnedIn += HandleOnEntitySpawnedIn;
		}

		public unsafe void OnFrameworkDestroyed()
		{
			healthTraker.OnEntityHealthChanged -= HandleOnEntityHealthChanged;
			spawnDispatcher.OnEntitySpawnedIn -= HandleOnEntitySpawnedIn;
			_eqObserver.RemoveAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_notificationObserver.RemoveAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_progressObserver.RemoveAction(new ObserverAction<TeamBaseStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		internal void RegisterView(IBattleArenaHUDView view)
		{
			_views.Add(view);
		}

		private void HandleOnEqualizerNotificationReceived(ref EqualizerNotificationDependency parameter)
		{
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			TeamType teamType = GetTeamType(parameter.TeamID);
			EqualizerNotification equalizerNotific = parameter.EqualizerNotific;
			switch (equalizerNotific)
			{
			case EqualizerNotification.Activate:
			{
				int hudId = 0;
				for (int i = 0; i < _views.get_Count(); i++)
				{
					IBattleArenaHUDView view = _views.get_Item(i);
					float progress = 0f;
					if (parameter.Health > 0)
					{
						float num = (float)parameter.Health / (float)parameter.MaxHealth;
						progress = 1f - num;
					}
					view.SetPercent(TargetType.EqualizerCrystal, hudId, progress);
					view.SetColor(TargetType.EqualizerCrystal, hudId, teamType);
					view.Scale(TargetType.EqualizerCrystal, hudId, Vector3.get_zero());
					view.SetItemEnabled(TargetType.EqualizerCrystal, hudId, enabled: true);
					view.PlaySpawnAnimation(TargetType.EqualizerCrystal, hudId, teamType, delegate
					{
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						view.Scale(TargetType.EqualizerCrystal, hudId, Vector3.get_one());
					});
				}
				TaskRunner.get_Instance().Run(UpdateEqualizerDuration(parameter.Time));
				break;
			}
			case EqualizerNotification.Deactivated:
				SetEqualiserDeactivated("strCoreLost", enabled: false, teamType, playDisappearAnimation: true);
				break;
			case EqualizerNotification.Defended:
				SetEqualiserDeactivated("strCoreDefended", enabled: false, teamType, playDisappearAnimation: true);
				break;
			case EqualizerNotification.Destroyed:
				SetEqualiserDeactivated("strCoreDestroyed", enabled: false, teamType, playDisappearAnimation: false);
				break;
			}
		}

		private void SetEqualiserDeactivated(string strKey, bool enabled, TeamType teamType, bool playDisappearAnimation)
		{
			for (int i = 0; i < _views.get_Count(); i++)
			{
				IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
				if (playDisappearAnimation)
				{
					battleArenaHUDView.PlayDisappearAnimation(TargetType.EqualizerCrystal, 0, teamType);
				}
				else
				{
					battleArenaHUDView.PlayExplodeAnimation(TargetType.EqualizerCrystal, 0, teamType);
				}
				battleArenaHUDView.SetItemEnabled(TargetType.EqualizerCrystal, 0, enabled: false);
				battleArenaHUDView.SetText(TargetType.EqualizerCrystal, 1, StringTableBase<StringTable>.Instance.GetString(strKey));
				battleArenaHUDView.SetColor(TargetType.EqualizerCrystal, 1, teamType);
				battleArenaHUDView.SetItemEnabled(TargetType.EqualizerCrystal, 1, enabled: true);
				battleArenaHUDView.PlayAnimation(TargetType.EqualizerCrystal, 1);
			}
		}

		private void HandleOnCapturePointProgressChanged(ref TeamBaseStateDependency parameter)
		{
			int capturePointVisualId = GetCapturePointVisualId(parameter.team);
			for (int i = 0; i < _views.get_Count(); i++)
			{
				_views.get_Item(i).SetPercent(TargetType.CapturePoint, capturePointVisualId, parameter.currentProgress / parameter.maxProgress);
			}
		}

		private void HandleOnCapturePointNotificationReceived(ref CapturePointNotificationDependency parameter)
		{
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			int capturePointVisualId = GetCapturePointVisualId(parameter.id);
			switch (parameter.notification)
			{
			case CapturePointNotification.CaptureLocked:
			case CapturePointNotification.CaptureUnlocked:
				break;
			case CapturePointNotification.SegmentCompleted:
				break;
			case CapturePointNotification.CaptureStarted:
				PlayCapturePointBounceAnimation(capturePointVisualId);
				break;
			case CapturePointNotification.CaptureStoppedNoAttackers:
			case CapturePointNotification.CaptureStoppedByDefenders:
				StopCapturePointBounceAnimation(capturePointVisualId);
				break;
			case CapturePointNotification.CaptureCompleted:
				StopCapturePointBounceAnimation(capturePointVisualId);
				for (int j = 0; j < _views.get_Count(); j++)
				{
					IBattleArenaHUDView battleArenaHUDView2 = _views.get_Item(j);
					battleArenaHUDView2.SetColorWithAnimation(TargetType.CapturePoint, capturePointVisualId, GetTeamType(parameter.attackingTeam));
				}
				UpdateTeamBasePulse(parameter.attackingTeam);
				UpdateTeamBasePulse(parameter.defendingTeam);
				break;
			case CapturePointNotification.Dominating:
				UpdateTeamBasePulse(parameter.attackingTeam, 4);
				if (playerTeamsContainer.IsMyTeam(parameter.attackingTeam))
				{
					for (int k = 0; k < _views.get_Count(); k++)
					{
						IBattleArenaHUDView battleArenaHUDView3 = _views.get_Item(k);
						battleArenaHUDView3.SetText(TargetType.EqualizerCrystal, 1, StringTableBase<StringTable>.Instance.GetString("strDominating"));
						battleArenaHUDView3.SetColor(TargetType.EqualizerCrystal, 1, TeamType.Friend);
						battleArenaHUDView3.SetItemEnabled(TargetType.EqualizerCrystal, 1, enabled: true);
						battleArenaHUDView3.PlayAnimation(TargetType.EqualizerCrystal, 1);
					}
				}
				break;
			case CapturePointNotification.Spawn:
			{
				Vector3 position = machineRootContainer.GetMachineRoot(TargetType.CapturePoint, parameter.id).get_transform().get_position();
				TeamType teamType = GetTeamType(parameter.defendingTeam);
				for (int i = 0; i < _views.get_Count(); i++)
				{
					IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
					battleArenaHUDView.RegisterItem(TargetType.CapturePoint, capturePointVisualId, Vector3.get_zero(), position);
					if (teamType != TeamType.Neutral)
					{
						battleArenaHUDView.SetColor(TargetType.CapturePoint, capturePointVisualId, teamType);
					}
				}
				UpdateTeamBasePulse(parameter.defendingTeam);
				break;
			}
			}
		}

		private void PlayCapturePointBounceAnimation(int id)
		{
			for (int i = 0; i < _views.get_Count(); i++)
			{
				IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
				battleArenaHUDView.PlayBounceAnimation(TargetType.CapturePoint, id);
			}
		}

		private void StopCapturePointBounceAnimation(int id)
		{
			for (int i = 0; i < _views.get_Count(); i++)
			{
				IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
				battleArenaHUDView.StopBounceAnimation(TargetType.CapturePoint, id);
			}
		}

		private void HandleOnEntitySpawnedIn(SpawnInParametersEntity spawnInParameters)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			int id = spawnInParameters.EntityID;
			if (spawnInParameters.Type == TargetType.TeamBase)
			{
				id = ((!spawnInParameters.IsOnMyTeam) ? 1 : 0);
			}
			Transform transform = machineRootContainer.GetMachineRoot(spawnInParameters.Type, spawnInParameters.EntityID).get_transform();
			Vector3 position = transform.get_position() + transform.get_rotation() * spawnInParameters.PreloadedMachine.machineInfo.MachineCenter;
			Vector3 machineSize = spawnInParameters.PreloadedMachine.machineInfo.MachineSize;
			for (int i = 0; i < _views.get_Count(); i++)
			{
				IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
				battleArenaHUDView.RegisterItem(spawnInParameters.Type, id, machineSize, position);
				if (spawnInParameters.Type == TargetType.EqualizerCrystal)
				{
					battleArenaHUDView.SetItemEnabled(spawnInParameters.Type, 0, enabled: false);
				}
			}
		}

		private void HandleOnEntityHealthChanged(TargetType type, int id, float currentPercent, float deltaPercent)
		{
			if (type == TargetType.TeamBase)
			{
				id = ((battlePlayers.MyTeam != id) ? 1 : 0);
			}
			if (type == TargetType.EqualizerCrystal)
			{
				currentPercent = 1f - currentPercent;
			}
			for (int i = 0; i < _views.get_Count(); i++)
			{
				IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
				battleArenaHUDView.SetPercent(type, id, currentPercent);
				int playerTeam = playerTeamsContainer.GetPlayerTeam(type, id);
				if (type == TargetType.EqualizerCrystal)
				{
					TeamType teamType = GetTeamType(playerTeam);
					battleArenaHUDView.PlayHitParticle(type, 0, teamType);
				}
			}
		}

		private IEnumerator UpdateEqualizerDuration(int eqDurationSeconds)
		{
			while (eqDurationSeconds > 0)
			{
				eqDurationSeconds--;
				for (int i = 0; i < _views.get_Count(); i++)
				{
					IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
					battleArenaHUDView.SetText(TargetType.EqualizerCrystal, 0, $":{eqDurationSeconds}");
				}
				yield return (object)new WaitForSecondsEnumerator(1f);
			}
		}

		private TeamType GetTeamType(int ownerTeam)
		{
			if (ownerTeam == -1)
			{
				return TeamType.Neutral;
			}
			return (ownerTeam != battlePlayers.MyTeam) ? TeamType.Enemy : TeamType.Friend;
		}

		private int GetCapturePointVisualId(int id)
		{
			if (id == 0 && battlePlayers.MyTeam == 1)
			{
				return 2;
			}
			if (id == 2 && battlePlayers.MyTeam == 1)
			{
				return 0;
			}
			return id;
		}

		private void UpdateTeamBasePulse(int team)
		{
			if (team != -1)
			{
				int playersCountOnTeam = playerTeamsContainer.GetPlayersCountOnTeam(TargetType.CapturePoint, team);
				UpdateTeamBasePulse(team, playersCountOnTeam);
			}
		}

		private void UpdateTeamBasePulse(int team, int speed)
		{
			int id = (battlePlayers.MyTeam != team) ? 1 : 0;
			for (int i = 0; i < _views.get_Count(); i++)
			{
				IBattleArenaHUDView battleArenaHUDView = _views.get_Item(i);
				battleArenaHUDView.SetPulseSpeed(TargetType.TeamBase, id, speed);
			}
		}
	}
}
