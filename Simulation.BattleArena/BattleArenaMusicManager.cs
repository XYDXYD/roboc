using Simulation.BattleArena.CapturePoint;
using Simulation.BattleArena.Equalizer;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Simulation.BattleArena
{
	internal sealed class BattleArenaMusicManager : IInitialize, IWaitForFrameworkDestruction, IMusicManager
	{
		private EqualizerNotificationObserver _eqObserver;

		private CapturePointNotificationObserver _notificationObserver;

		private MusicManagerView _view;

		private float[] _baseHealth = new float[2];

		private bool _endGame;

		private bool _equalizer;

		[Inject]
		internal MasterVolumeController masterVolumeController
		{
			private get;
			set;
		}

		[Inject]
		internal HealthTracker healthTracker
		{
			private get;
			set;
		}

		[Inject]
		internal MultiplayerGameTimerClient multiplayerGameTimer
		{
			private get;
			set;
		}

		public BattleArenaMusicManager(EqualizerNotificationObserver _eqObserver, CapturePointNotificationObserver _notificationObserver)
		{
			this._eqObserver = _eqObserver;
			this._notificationObserver = _notificationObserver;
		}

		public void OnDependenciesInjected()
		{
			healthTracker.OnEntityHealthChanged += HandleOnEntityHealthChanged;
			multiplayerGameTimer.OnTimeUpdated += OnTimerUpdated;
		}

		public void OnFrameworkDestroyed()
		{
			healthTracker.OnEntityHealthChanged -= HandleOnEntityHealthChanged;
			multiplayerGameTimer.OnTimeUpdated -= OnTimerUpdated;
		}

		public unsafe void SetView(MusicManagerView musicManagerView)
		{
			_view = musicManagerView;
			if (masterVolumeController.GetMusicVolume() > 0f)
			{
				_view.StartMusic();
				_eqObserver.AddAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				_notificationObserver.AddAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		public void SwitchToVotingScreenLoop()
		{
			_view.SwitchToVotingScreenLoop();
		}

		private unsafe void OnTimerUpdated(float currenTime)
		{
			if (currenTime < 60f && currenTime > 0f)
			{
				if (!_endGame)
				{
					_view.SwitchToEndGameLoop();
				}
				healthTracker.OnEntityHealthChanged -= HandleOnEntityHealthChanged;
				_eqObserver.RemoveAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				multiplayerGameTimer.OnTimeUpdated -= OnTimerUpdated;
			}
		}

		private void HandleOnEntityHealthChanged(TargetType type, int id, float percent, float arg4)
		{
			if (type == TargetType.TeamBase)
			{
				_baseHealth[id] = percent;
				CheckBaseHealth();
			}
		}

		private void CheckBaseHealth()
		{
			if (_baseHealth[0] >= 0.95f || ((double)_baseHealth[1] >= 0.95 && !_endGame))
			{
				_endGame = true;
				_view.SwitchToEndGameLoop();
			}
			else if (_baseHealth[0] < 0.95f && (double)_baseHealth[1] < 0.95 && _endGame)
			{
				_endGame = false;
				if (_equalizer)
				{
					_view.SwitchToSecondaryLoop();
				}
				else
				{
					_view.SwitchToMainLoop();
				}
			}
		}

		private unsafe void HandleOnCapturePointNotificationReceived(ref CapturePointNotificationDependency parameter)
		{
			if (parameter.notification == CapturePointNotification.CaptureCompleted)
			{
				_view.SwitchToMainLoop();
				_notificationObserver.RemoveAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void HandleOnEqualizerNotificationReceived(ref EqualizerNotificationDependency parameter)
		{
			switch (parameter.EqualizerNotific)
			{
			case EqualizerNotification.Activate:
				_equalizer = true;
				if (!_endGame)
				{
					_view.SwitchToSecondaryLoop();
				}
				break;
			case EqualizerNotification.Deactivated:
			case EqualizerNotification.Defended:
			case EqualizerNotification.Destroyed:
				_equalizer = false;
				if (!_endGame)
				{
					_view.SwitchToMainLoop();
				}
				break;
			}
		}
	}
}
