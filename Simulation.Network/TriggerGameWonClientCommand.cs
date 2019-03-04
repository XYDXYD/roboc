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
	internal sealed class TriggerGameWonClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameWonDependency _dependency;

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

		[Inject]
		internal VOManager voManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as GameWonDependency);
			return this;
		}

		public void Execute()
		{
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Won, _dependency.gameEndReason);
			gameEndedObserver.GameEnded(won: true);
			IServiceRequest serviceRequest = serviceFactory.Create<ISaveLastMatchResultRequest, bool>(param: true);
			serviceRequest.Execute();
			PlayVictoryGui();
		}

		private void PlayVictoryGui()
		{
			if (WorldSwitching.GetGameModeType() != GameModeType.Campaign)
			{
				GameObject val = gameObjectFactory.Build("HUD_Victory");
				TaskRunner.get_Instance().Run(ExitOnAnimationEnd(val.GetComponent<Animation>()));
			}
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_PlayerVictory", 0);
			voManager.PlayVO(AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_Victory));
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
