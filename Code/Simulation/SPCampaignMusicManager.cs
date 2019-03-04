using Simulation.SinglePlayerCampaign;
using Simulation.SinglePlayerCampaign.DataTypes;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Simulation
{
	internal class SPCampaignMusicManager : IInitialize, IWaitForFrameworkDestruction, IMusicManager
	{
		private MusicManagerView _musicManagerView;

		private bool _isTheLastWave;

		private bool _mainLoopIsPlaying;

		[Inject]
		internal CurrentWaveObserver currentWaveObserver
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
		internal MasterVolumeController masterVolumeController
		{
			private get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			currentWaveObserver.AddAction(new ObserverAction<WaveCounterInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			destructionReporter.OnPlayerDamageApplied += SwitchToMainLoop;
		}

		public unsafe void OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerDamageApplied -= SwitchToMainLoop;
			currentWaveObserver.RemoveAction(new ObserverAction<WaveCounterInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void SetView(MusicManagerView musicManagerView)
		{
			_musicManagerView = musicManagerView;
			if (masterVolumeController.GetMusicVolume() > 0f)
			{
				_musicManagerView.StartMusic();
				_mainLoopIsPlaying = false;
			}
		}

		public void SwitchToVotingScreenLoop()
		{
		}

		private void SwitchToMainLoop(DestructionData obj)
		{
			if (!_mainLoopIsPlaying && !_isTheLastWave)
			{
				_mainLoopIsPlaying = true;
				_musicManagerView.SwitchToMainLoop();
			}
		}

		private void SwitchToEndGameLoop(ref WaveCounterInfo waveCounterInfo)
		{
			_isTheLastWave = (waveCounterInfo.WaveJustStartedIndex == waveCounterInfo.NumberOfWaves - 1);
			if (_isTheLastWave)
			{
				_musicManagerView.SwitchToEndGameLoop();
				_mainLoopIsPlaying = false;
			}
		}
	}
}
