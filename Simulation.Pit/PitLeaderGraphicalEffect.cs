using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Pit
{
	internal class PitLeaderGraphicalEffect : IInitialize, IWaitForFrameworkDestruction
	{
		private Dictionary<int, PitLeaderFX> _pitLeaderEffect = new Dictionary<int, PitLeaderFX>();

		private int _currentLeader = -1;

		private int _previousLeader = -1;

		private FasterList<ParticleSystemRenderer> _playerLeaderFXRenderers;

		[Inject]
		internal LivePlayersContainer livePlayerContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PitLeaderObserver pitLeaderObserver
		{
			private get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyContainer
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
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamContainer
		{
			private get;
			set;
		}

		[Inject]
		internal ZoomEngine zoomMode
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter reporter
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			spawnDispatcher.OnPlayerSpawnedIn += CreatePitLeaderEffect;
			PitLeaderObserver pitLeaderObserver = this.pitLeaderObserver;
			pitLeaderObserver.OnBecomingPitLeader = (Action<int>)Delegate.Combine(pitLeaderObserver.OnBecomingPitLeader, new Action<int>(HandleOnShowPitLeaderEffect));
			spawnDispatcher.OnPlayerRespawnedIn += HandlePitLeaderEffectOnRespawn;
			reporter.OnMachineKilled += HandlePitLeaderKilled;
			zoomMode.OnZoomModeChange += HandlePitLeaderEffectHideOnZoom;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			spawnDispatcher.OnPlayerSpawnedIn -= CreatePitLeaderEffect;
			PitLeaderObserver pitLeaderObserver = this.pitLeaderObserver;
			pitLeaderObserver.OnBecomingPitLeader = (Action<int>)Delegate.Remove(pitLeaderObserver.OnBecomingPitLeader, new Action<int>(HandleOnShowPitLeaderEffect));
			spawnDispatcher.OnPlayerRespawnedIn -= HandlePitLeaderEffectOnRespawn;
			zoomMode.OnZoomModeChange -= HandlePitLeaderEffectHideOnZoom;
			reporter.OnMachineKilled -= HandlePitLeaderKilled;
		}

		private void HandlePitLeaderEffectHideOnZoom(ZoomType zoomType, float zoomAmount)
		{
			if (zoomType == ZoomType.Zoomed)
			{
				for (int i = 0; i < _playerLeaderFXRenderers.get_Count(); i++)
				{
					if (_playerLeaderFXRenderers.get_Item(i) != null)
					{
						_playerLeaderFXRenderers.get_Item(i).set_enabled(false);
					}
				}
				_pitLeaderEffect[playerTeamContainer.localPlayerId].trailRend.set_time(-1f);
				return;
			}
			for (int j = 0; j < _playerLeaderFXRenderers.get_Count(); j++)
			{
				if (_playerLeaderFXRenderers.get_Item(j) != null)
				{
					_playerLeaderFXRenderers.get_Item(j).set_enabled(true);
				}
			}
			_pitLeaderEffect[playerTeamContainer.localPlayerId].trailRend.set_time(1f);
		}

		private void HandleOnShowPitLeaderEffect(int playerId)
		{
			if (_currentLeader == playerId)
			{
				return;
			}
			if (_currentLeader >= 0)
			{
				_previousLeader = _currentLeader;
				if (IsPlayerRespawning(_previousLeader))
				{
					_pitLeaderEffect[_previousLeader].SetLeader(isLeader: false);
				}
			}
			_currentLeader = playerId;
			if (_pitLeaderEffect[playerId] != null)
			{
				_pitLeaderEffect[playerId].SetLeader(isLeader: true);
			}
		}

		private bool IsPlayerRespawning(int player)
		{
			return livePlayerContainer.IsPlayerAlive(TargetType.Player, player);
		}

		private void HandlePitLeaderKilled(int ownerID, int shooterID)
		{
			if (_currentLeader == ownerID && _pitLeaderEffect[_currentLeader] != null)
			{
				_pitLeaderEffect[_currentLeader].StopLeaderFX();
			}
		}

		private void HandlePitLeaderEffectOnRespawn(SpawnInParametersPlayer spawnInParameters)
		{
			if (spawnInParameters.playerId == _previousLeader && _previousLeader != _currentLeader)
			{
				_pitLeaderEffect[_previousLeader].StopLeaderFXImmediately();
				_previousLeader = -1;
			}
			if (spawnInParameters.playerId == _currentLeader)
			{
				_pitLeaderEffect[_currentLeader].RestartEffect();
			}
		}

		private void CreatePitLeaderEffect(SpawnInParametersPlayer spawnInParameters)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectFactory.Build("PitLeaderFX");
			Transform transform = val.get_transform();
			Rigidbody rbData = spawnInParameters.preloadedMachine.rbData;
			transform.set_parent(rbData.get_transform());
			transform.set_localRotation(Quaternion.get_identity());
			transform.set_localPosition(rbData.get_centerOfMass());
			_pitLeaderEffect[spawnInParameters.playerId] = val.GetComponent<PitLeaderFX>();
			val.SetActive(true);
			int playerId = spawnInParameters.playerId;
			if (spawnInParameters.isMe)
			{
				_playerLeaderFXRenderers = new FasterList<ParticleSystemRenderer>((ICollection<ParticleSystemRenderer>)_pitLeaderEffect[playerId].GetComponentsInChildren<ParticleSystemRenderer>(true));
			}
		}
	}
}
