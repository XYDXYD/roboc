using Battle;
using Fabric;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeamBaseAudioGui : IInitialize, IWaitForFrameworkDestruction
	{
		private int _baseCaptureCount;

		private bool _loopIsPlaying;

		private const float RESET_WAIT_TIME = 2f;

		private float _myTeamResetTimer;

		private float _enemyResetTimer;

		[Inject]
		public NetworkMachineManager networkMachineManager
		{
			private get;
			set;
		}

		[Inject]
		public TeamBaseProgressDispatcher teamBaseProgressDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public GameEndedObserver gameEndedObserver
		{
			get;
			set;
		}

		[Inject]
		public BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			RegisterEventsForAllTeams();
			gameEndedObserver.OnGameEnded += OnGameEnded;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			UnregisterEventsForAllTeams();
			gameEndedObserver.OnGameEnded += OnGameEnded;
		}

		private void OnGameEnded(bool won)
		{
			UnregisterEventsForAllTeams();
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_averted_YourBase", 1);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_averted_EnemyBase", 1);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_sector_capture_YourBase", 1);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_sector_capture_EnemyBase", 1);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_all_capture_YourBase", 1);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_all_capture_EnemyBase", 1);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_prog_loop", 1);
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_prog_loop", 1);
		}

		private void RegisterEventsForAllTeams()
		{
			uint myTeam = battlePlayers.MyTeam;
			HashSet<int> allTeams = battlePlayers.GetAllTeams();
			foreach (int item in allTeams)
			{
				if (item == myTeam)
				{
					teamBaseProgressDispatcher.RegisterCaptureStarted(item, OnMyTeamCaptureStarted);
					teamBaseProgressDispatcher.RegisterCaptureStopped(item, OnMyTeamCaptureStopped);
					teamBaseProgressDispatcher.RegisterSectionComplete(item, OnMyTeamSectionCompleted);
					teamBaseProgressDispatcher.RegisterFinalSectionComplete(item, OnMyTeamFinalSectionCompleted);
					teamBaseProgressDispatcher.RegisterCaptureReset(item, OnMyTeamSectionReset);
				}
				else
				{
					teamBaseProgressDispatcher.RegisterCaptureStarted(item, OnEnemyTeamCaptureStarted);
					teamBaseProgressDispatcher.RegisterCaptureStopped(item, OnEnemyTeamCaptureStopped);
					teamBaseProgressDispatcher.RegisterSectionComplete(item, OnEnemyTeamSectionCompleted);
					teamBaseProgressDispatcher.RegisterFinalSectionComplete(item, OnEnemyTeamFinalSectionCompleted);
					teamBaseProgressDispatcher.RegisterCaptureReset(item, OnEnemyTeamSectionReset);
				}
			}
		}

		private void UnregisterEventsForAllTeams()
		{
			uint myTeam = battlePlayers.MyTeam;
			HashSet<int> allTeams = battlePlayers.GetAllTeams();
			foreach (int item in allTeams)
			{
				if (item == myTeam)
				{
					teamBaseProgressDispatcher.UnRegisterCaptureStarted(item, OnMyTeamCaptureStarted);
					teamBaseProgressDispatcher.UnRegisterCaptureStopped(item, OnMyTeamCaptureStopped);
					teamBaseProgressDispatcher.UnRegisterSectionComplete(item, OnMyTeamSectionCompleted);
					teamBaseProgressDispatcher.UnRegisterFinalSectionComplete(item, OnMyTeamFinalSectionCompleted);
					teamBaseProgressDispatcher.UnRegisterCaptureReset(item, OnMyTeamSectionReset);
				}
				else
				{
					teamBaseProgressDispatcher.UnRegisterCaptureStarted(item, OnEnemyTeamCaptureStarted);
					teamBaseProgressDispatcher.UnRegisterCaptureStopped(item, OnEnemyTeamCaptureStopped);
					teamBaseProgressDispatcher.UnRegisterSectionComplete(item, OnEnemyTeamSectionCompleted);
					teamBaseProgressDispatcher.UnRegisterFinalSectionComplete(item, OnEnemyTeamFinalSectionCompleted);
					teamBaseProgressDispatcher.UnRegisterCaptureReset(item, OnEnemyTeamSectionReset);
				}
			}
		}

		private void OnMyTeamSectionReset()
		{
			if (_myTeamResetTimer <= 0f)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)StartMyTeamTimer);
				EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_averted_YourBase", 0);
			}
		}

		private IEnumerator StartMyTeamTimer()
		{
			_myTeamResetTimer = 2f;
			while ((_myTeamResetTimer -= Time.get_deltaTime()) > 0f)
			{
				yield return null;
			}
		}

		private void OnEnemyTeamSectionReset()
		{
			if (_enemyResetTimer <= 0f)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)StartEnemyTimer);
				EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_averted_EnemyBase", 0);
			}
		}

		private IEnumerator StartEnemyTimer()
		{
			_enemyResetTimer = 2f;
			while ((_enemyResetTimer -= Time.get_deltaTime()) > 0f)
			{
				yield return null;
			}
		}

		private void OnMyTeamCaptureStarted()
		{
			_baseCaptureCount++;
			UpdatePlayLoop();
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_sector_capture_YourBase", 0);
		}

		private void OnEnemyTeamCaptureStarted()
		{
			_baseCaptureCount++;
			UpdatePlayLoop();
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_sector_capture_EnemyBase", 0);
		}

		private void OnMyTeamCaptureStopped()
		{
			_baseCaptureCount--;
			UpdatePlayLoop();
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_averted_YourBase", 0);
		}

		private void OnEnemyTeamCaptureStopped()
		{
			_baseCaptureCount--;
			UpdatePlayLoop();
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_averted_EnemyBase", 0);
		}

		private void OnMyTeamSectionCompleted()
		{
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_sector_capture_YourBase", 0);
		}

		private void OnEnemyTeamSectionCompleted()
		{
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_sector_capture_EnemyBase", 0);
		}

		private void OnMyTeamFinalSectionCompleted()
		{
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_all_capture_YourBase", 0);
		}

		private void OnEnemyTeamFinalSectionCompleted()
		{
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_all_capture_EnemyBase", 0);
		}

		private void UpdatePlayLoop()
		{
			if (_baseCaptureCount > 0)
			{
				if (!_loopIsPlaying)
				{
					EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_prog_loop", 0);
					_loopIsPlaying = true;
				}
			}
			else if (_loopIsPlaying)
			{
				EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_base_capture_prog_loop", 1);
				_loopIsPlaying = false;
			}
		}
	}
}
