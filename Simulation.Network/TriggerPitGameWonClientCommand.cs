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
	internal sealed class TriggerPitGameWonClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameWonDependency _dependency;

		[Inject]
		public ICommandFactory commandFactory
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
		public PlayerNamesContainer playerNamesContainer
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
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Won, _dependency.gameEndReason);
			gameEndedObserver.GameEnded(won: true);
			IServiceRequest serviceRequest = serviceFactory.Create<ISaveLastMatchResultRequest, bool>(param: true);
			serviceRequest.Execute();
			PlayVictoryGui();
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as GameWonDependency);
			return this;
		}

		private void PlayVictoryGui()
		{
			GameObject val = gameObjectFactory.Build("HUD_PIT_Complete");
			val.GetComponent<VictoryLabelSetter>().SetVictoryLabel(StringTableBase<StringTable>.Instance.GetString("strYouWereVictorious"));
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Pit_End_Victory), 0);
			TaskRunner.get_Instance().Run(ExitOnAnimationEnd(val.GetComponent<Animation>()));
		}

		private IEnumerator ExitOnAnimationEnd(Animation animation)
		{
			while (animation.get_isPlaying())
			{
				yield return null;
			}
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Gui_LevelUp_Close), 0);
			guiInputController.ShowScreen(GuiScreens.BattleStatsScreen);
		}
	}
}
