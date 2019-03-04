using Fabric;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using Svelto.Ticker.Legacy;
using System;
using System.Collections;

namespace Simulation
{
	internal sealed class SurrenderVotePresenter : IInitialize, IWaitForFrameworkDestruction, ITickable, IHandleCharacterInput, ITickableBase, IInputComponent, IComponent
	{
		private SurrenderVoteView _surrenderVoteView;

		private int inputCalled;

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal SurrenderControllerClient surrenderControllerClient
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			surrenderControllerClient.OnNumVotesChanged += HandleOnNumVotesChanged;
			surrenderControllerClient.ShowSurrenderGUI += ShowSurrenderGUI;
			surrenderControllerClient.HideSurrenderGUI += HideSurrenderGUI;
			surrenderControllerClient.ShowPlayerVotedGUI += ShowPlayerVotedGUI;
			surrenderControllerClient.ShowPlayerInitiatedVoteGUI += ShowPlayerInitiatedVoteGUI;
			surrenderControllerClient.RegisterOnVoteTimerFinished(HandleOnVoteTimerFinished);
			surrenderControllerClient.RegisterOnVoteTimerChanged(HandleOnVoteTimeChanged);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			surrenderControllerClient.OnNumVotesChanged -= HandleOnNumVotesChanged;
			surrenderControllerClient.ShowSurrenderGUI -= ShowSurrenderGUI;
			surrenderControllerClient.HideSurrenderGUI -= HideSurrenderGUI;
			surrenderControllerClient.ShowPlayerVotedGUI -= ShowPlayerVotedGUI;
			surrenderControllerClient.ShowPlayerInitiatedVoteGUI -= ShowPlayerInitiatedVoteGUI;
			surrenderControllerClient.UnregisterOnVoteTimerFinished(HandleOnVoteTimerFinished);
			surrenderControllerClient.UnregisterOnVoteTimerChanged(HandleOnVoteTimeChanged);
		}

		public void HandleCharacterInput(InputCharacterData data)
		{
			if (!surrenderControllerClient._voteCast)
			{
				if (data.data[19] > 0f)
				{
					inputCalled++;
					surrenderControllerClient._voteCast = true;
					PlayerVoted(vote: true);
				}
				else if (data.data[20] > 0f)
				{
					inputCalled++;
					surrenderControllerClient._voteCast = true;
					PlayerVoted(vote: false);
				}
			}
		}

		public void RegisterView(SurrenderVoteView surrenderVoteView)
		{
			_surrenderVoteView = surrenderVoteView;
		}

		public void UnregisterView(SurrenderVoteView surrenderVoteView)
		{
			_surrenderVoteView = null;
		}

		public void Tick(float deltaTime)
		{
			surrenderControllerClient.UpdateVoteTimer(deltaTime);
		}

		private void PlayerVoted(bool vote)
		{
			commandFactory.Build<SurrenderVoteCastClientCommand>().Inject(vote).Execute();
			surrenderControllerClient.PlayerCastVote();
			ShowPlayerVotedGUI();
		}

		private void HandleOnNumVotesChanged(FasterList<bool> votes, int numNewVotes)
		{
			_surrenderVoteView.UpdateVotes(votes, numNewVotes);
			if (votes.get_Item(votes.get_Count() - 1))
			{
				EventManager.get_Instance().PostEvent("GUI_Surrender_Yes", 0);
			}
			else
			{
				EventManager.get_Instance().PostEvent("GUI_Surrender_No", 0);
			}
		}

		private void HandleOnVoteTimeChanged(double currentTime)
		{
			if (!surrenderControllerClient.HasPlayerVoted())
			{
				_surrenderVoteView.UpdateTimer(currentTime);
			}
		}

		private void HandleOnVoteTimerFinished()
		{
			if (!surrenderControllerClient.HasPlayerVoted())
			{
				PlayerVoted(vote: false);
			}
		}

		private void ShowSurrenderGUI(int numPlayers)
		{
			_surrenderVoteView.ShowTimer();
			_surrenderVoteView.Show(numPlayers);
			EventManager.get_Instance().PostEvent("GUI_Surrender_Request", 0);
		}

		private void HideSurrenderGUI()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HideVoteView);
		}

		private IEnumerator HideVoteView()
		{
			yield return (object)new WaitForSecondsEnumerator(2f);
			EventManager.get_Instance().PostEvent("GUI_Surrender_Declined", 0);
			_surrenderVoteView.Hide();
		}

		private void ShowPlayerVotedGUI()
		{
			_surrenderVoteView.PlayerVoted();
		}

		private void ShowPlayerInitiatedVoteGUI()
		{
			_surrenderVoteView.PlayerInitiatedVote();
		}
	}
}
