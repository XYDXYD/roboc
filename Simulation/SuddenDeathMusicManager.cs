using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal class SuddenDeathMusicManager : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IMusicManager
	{
		private const int MAX_SECTIONS = 4;

		private bool _gameEnd;

		private bool _mainLoop;

		private int[] _sectionCompleted = new int[2];

		private MusicManagerView _view;

		[Inject]
		internal MasterVolumeController masterVolumeController
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

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer teamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal TeamBaseProgressDispatcher teamBaseProgressDispatcher
		{
			private get;
			set;
		}

		public void OnFrameworkInitialized()
		{
			if (_view != null)
			{
				Register();
			}
		}

		private void Register()
		{
			multiplayerGameTimer.OnTimeUpdated += OnTimerUpdated;
			destructionReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
			teamBaseProgressDispatcher.RegisterSectionComplete(0, HandleOnSectionComplete0);
			teamBaseProgressDispatcher.RegisterSectionComplete(1, HandleOnSectionComplete1);
			teamBaseProgressDispatcher.RegisterCaptureStarted(0, HandleOnCaptureStarted);
			teamBaseProgressDispatcher.RegisterCaptureStarted(1, HandleOnCaptureStarted);
		}

		public void OnFrameworkDestroyed()
		{
			if (_view != null)
			{
				destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
				UnregisterOnGameEnd();
			}
		}

		public void SetView(MusicManagerView musicManagerView)
		{
			_view = musicManagerView;
			if (masterVolumeController.GetMusicVolume() > 0f)
			{
				_view.StartMusic();
			}
		}

		public void SwitchToVotingScreenLoop()
		{
			_view.SwitchToVotingScreenLoop();
		}

		private void UnregisterOnGameEnd()
		{
			if (!_gameEnd)
			{
				_gameEnd = true;
				multiplayerGameTimer.OnTimeUpdated -= OnTimerUpdated;
				destructionReporter.OnMachineDestroyed -= HandleOnMachineDestroyed;
				teamBaseProgressDispatcher.UnRegisterSectionComplete(0, HandleOnSectionComplete0);
				teamBaseProgressDispatcher.UnRegisterSectionComplete(1, HandleOnSectionComplete1);
			}
		}

		private void HandleOnSectionComplete0()
		{
			HandleOnSectionComplete(0);
		}

		private void HandleOnSectionComplete1()
		{
			HandleOnSectionComplete(1);
		}

		private void HandleOnSectionComplete(int team)
		{
			_sectionCompleted[team]++;
			if (4 - _sectionCompleted[team] == 1)
			{
				_view.SwitchToEndGameLoop();
				UnregisterOnGameEnd();
			}
		}

		private void HandleOnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			if (_view == null)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			List<int> livePlayers = livePlayersContainer.GetLivePlayers(TargetType.Player);
			for (int i = 0; i < livePlayers.Count; i++)
			{
				int player = livePlayers[i];
				if (teamsContainer.GetPlayerTeam(TargetType.Player, player) == 0)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
			if (num == 1 || num2 == 1)
			{
				_view.SwitchToEndGameLoop();
				UnregisterOnGameEnd();
			}
		}

		private void HandleOnPlayerDamageApplied(DestructionData _)
		{
			TrySwitchToMainLoop();
		}

		private void HandleOnCaptureStarted()
		{
			TrySwitchToMainLoop();
		}

		private void TrySwitchToMainLoop()
		{
			if (!_mainLoop && !_gameEnd)
			{
				_mainLoop = true;
				_view.SwitchToMainLoop();
				teamBaseProgressDispatcher.UnRegisterCaptureStarted(0, HandleOnCaptureStarted);
				teamBaseProgressDispatcher.UnRegisterCaptureStarted(1, HandleOnCaptureStarted);
				destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
			}
		}

		private void OnTimerUpdated(float currenTime)
		{
			if (currenTime < 60f)
			{
				_view.SwitchToEndGameLoop();
				UnregisterOnGameEnd();
			}
		}
	}
}
