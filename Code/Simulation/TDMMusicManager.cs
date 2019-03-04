using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal class TDMMusicManager : IInitialize, IWaitForFrameworkDestruction, IMusicManager
	{
		private int _killLimit = 30;

		private bool _gameEnd;

		private bool _mainLoop;

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

		public void OnDependenciesInjected()
		{
			multiplayerGameTimer.OnTimeUpdated += OnTimerUpdated;
		}

		public void OnFrameworkDestroyed()
		{
			multiplayerGameTimer.OnTimeUpdated -= OnTimerUpdated;
		}

		private void OnTimerUpdated(float currenTime)
		{
			if (currenTime < 60f && currenTime > 0f && !_gameEnd)
			{
				_gameEnd = true;
				_view.SwitchToEndGameLoop();
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

		internal void SetKillLimit(int killLimit)
		{
			_killLimit = killLimit;
		}

		internal void UpdateKills(int value)
		{
			if (!_gameEnd)
			{
				if (value == 1 && !_mainLoop)
				{
					_mainLoop = true;
					_view.SwitchToMainLoop();
				}
				if (_killLimit - value == 1)
				{
					_gameEnd = true;
					_view.SwitchToEndGameLoop();
				}
			}
		}
	}
}
