using Svelto.IoC;

namespace Simulation
{
	internal class PitModeMusicManager : IMusicManager
	{
		private const uint PITMODE_SCORE_TRESHOLD = 16u;

		private bool _gameEnd;

		private bool _mainLoop;

		private MusicManagerView _view;

		[Inject]
		internal MasterVolumeController masterVolumeController
		{
			private get;
			set;
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

		internal void UpdateScores(int value)
		{
			if (!_gameEnd)
			{
				if (value >= 1 && !_mainLoop)
				{
					_mainLoop = true;
					_view.SwitchToMainLoop();
				}
				if (16L - (long)value <= 1)
				{
					_gameEnd = true;
					_view.SwitchToEndGameLoop();
				}
			}
		}
	}
}
