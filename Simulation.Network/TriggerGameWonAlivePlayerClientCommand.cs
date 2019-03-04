using Fabric;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using UnityEngine;

namespace Simulation.Network
{
	internal sealed class TriggerGameWonAlivePlayerClientCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		public ICommandFactory commandFactory
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
		public LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		public GameStateClient gameStateClient
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public void Execute()
		{
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Won);
			gameEndedObserver.GameEnded(won: true);
			IServiceRequest serviceRequest = serviceFactory.Create<ISaveLastMatchResultRequest, bool>(param: true);
			serviceRequest.Execute();
			PlayVictoryGui();
		}

		private void PlayVictoryGui()
		{
			GameObject val = gameObjectFactory.Build("HUD_Victory");
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_PlayerVictory", 0);
			TaskRunner.get_Instance().Run(ExitOnAnimationEnd(val.GetComponent<Animation>()));
		}

		private IEnumerator ExitOnAnimationEnd(Animation animation)
		{
			while (animation.get_isPlaying())
			{
				yield return null;
			}
			guiInputController.ShowScreen(GuiScreens.BattleStatsScreen);
		}
	}
}
