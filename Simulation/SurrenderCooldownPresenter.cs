using Svelto.Context;
using Svelto.IoC;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal sealed class SurrenderCooldownPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private SurrenderCooldownView _surrenderCooldownView;

		[Inject]
		internal SurrenderControllerClient surrenderControllerClient
		{
			private get;
			set;
		}

		[Inject]
		internal GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			surrenderControllerClient.ShowSurrenderGUI += ShowSurrenderGUI;
			surrenderControllerClient.RegisterOnCooldownSecondsChanged(HandleOnCooldownSecondsChanged);
			surrenderControllerClient.RegisterOnCooldownTimerStarted(HandleOnCooldownTimerStarted);
			surrenderControllerClient.RegisterOnCooldownTimerEnded(HandleOnCooldownTimerEnded);
			gameStartDispatcher.Register(StartCountdown);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			surrenderControllerClient.ShowSurrenderGUI -= ShowSurrenderGUI;
			surrenderControllerClient.UnregisterOnCooldownSecondsChanged(HandleOnCooldownSecondsChanged);
			surrenderControllerClient.UnregisterOnCooldownTimerStarted(HandleOnCooldownTimerStarted);
			surrenderControllerClient.UnregisterOnCooldownTimerEnded(HandleOnCooldownTimerEnded);
			gameStartDispatcher.Unregister(StartCountdown);
		}

		private void StartCountdown()
		{
			TaskRunnerExtensions.Run(Tick());
		}

		public void RegisterView(SurrenderCooldownView surrenderVoteView)
		{
			_surrenderCooldownView = surrenderVoteView;
		}

		public void UnregisterView(SurrenderCooldownView surrenderVoteView)
		{
			_surrenderCooldownView = null;
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				surrenderControllerClient.UpdateCooldownTimer(Time.get_deltaTime());
				yield return null;
			}
		}

		private void HandleOnCooldownSecondsChanged(int currentSecondsLeft)
		{
			_surrenderCooldownView.UpdateCooldownTimeLeft(currentSecondsLeft);
		}

		private void HandleOnCooldownTimerEnded()
		{
			_surrenderCooldownView.FlipCooldownTimerVisibility();
		}

		private void HandleOnCooldownTimerStarted()
		{
		}

		private void ShowSurrenderGUI(int numPlayers)
		{
			_surrenderCooldownView.ShowVoteInProgress();
		}
	}
}
