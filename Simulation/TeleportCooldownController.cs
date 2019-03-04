using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeleportCooldownController : IInitialize, IWaitForFrameworkDestruction, ITeleportCooldownController
	{
		private const float COOLDOWN_TIME = 10f;

		private float _lastDamageTime = -10f;

		private bool _isDeactivated;

		[Inject]
		public DestructionReporter destructionReporter
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
		public LobbyGameStartPresenter lobbyPresenter
		{
			private get;
			set;
		}

		[Inject]
		public ISpectatorModeActivator spectatorModeActivator
		{
			private get;
			set;
		}

		[Inject]
		public MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		public event Action OnCooldownRestart = delegate
		{
		};

		public event Action OnTeleportAttemptStart = delegate
		{
		};

		public event Action OnTeleportAttemptEnd = delegate
		{
		};

		public event Action OnCooldownDeactivated = delegate
		{
		};

		public float GetCooldownTime()
		{
			return 10f;
		}

		public bool TeleportIsAllowed()
		{
			if (!WorldSwitching.IsMultiplayer())
			{
				return true;
			}
			if (_isDeactivated)
			{
				return true;
			}
			if (Time.get_time() > _lastDamageTime + 10f)
			{
				return true;
			}
			return false;
		}

		public void TeleportAttemptStarted()
		{
			this.OnTeleportAttemptStart();
		}

		public void TeleportAttemptEnded()
		{
			this.OnTeleportAttemptEnd();
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnPlayerDamageApplied += OnDamage;
			LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
			lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Combine(lobbyPresenter.OnInitialLobbyGuiClose, new Action(OnBattleLobbyClosed));
			if (WorldSwitching.GetGameModeType() != GameModeType.Pit)
			{
				spectatorModeActivator.Register(OnSpectatorMode);
			}
			if (WorldSwitching.IsMultiplayer())
			{
				spawnDispatcher.OnPlayerRespawnedIn += HandleOnRespawnedIn;
			}
		}

		private void HandleOnRespawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (spawnInParameters.isMe)
			{
				_lastDamageTime = Time.get_time();
				_isDeactivated = false;
				this.OnCooldownRestart();
			}
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			if (destructionReporter != null)
			{
				destructionReporter.OnPlayerDamageApplied -= OnDamage;
			}
			if (this.lobbyPresenter != null)
			{
				LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
				lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Remove(lobbyPresenter.OnInitialLobbyGuiClose, new Action(OnBattleLobbyClosed));
			}
			if (spectatorModeActivator != null)
			{
				spectatorModeActivator.Unregister(OnSpectatorMode);
			}
		}

		private void OnSpectatorMode(int killer, bool enabled)
		{
			_isDeactivated = true;
			this.OnCooldownDeactivated();
		}

		private void OnBattleLobbyClosed()
		{
			_lastDamageTime = Time.get_time();
			this.OnCooldownRestart();
			LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
			lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Remove(lobbyPresenter.OnInitialLobbyGuiClose, new Action(OnBattleLobbyClosed));
		}

		private void OnDamage(DestructionData data)
		{
			if (data.targetIsMe)
			{
				_lastDamageTime = Time.get_time();
				this.OnCooldownRestart();
			}
		}

		private void OnDestruction(FasterList<InstantiatedCube> cubes)
		{
			_lastDamageTime = Time.get_time();
			this.OnCooldownRestart();
		}
	}
}
