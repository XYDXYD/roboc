using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Network
{
	internal sealed class TriggerPitGameLostClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameLostDependency _dependency;

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
		public PlayerTeamsContainer teamsContainer
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
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Lost, _dependency.gameEndReason);
			gameEndedObserver.GameEnded(won: false);
			IServiceRequest serviceRequest = serviceFactory.Create<ISaveLastMatchResultRequest, bool>(param: false);
			serviceRequest.Execute();
			PlayLostGui();
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as GameLostDependency);
			return this;
		}

		private void PlayLostGui()
		{
			GameObject val = gameObjectFactory.Build("HUD_PIT_Complete");
			IList<int> playersOnTeam = teamsContainer.GetPlayersOnTeam(TargetType.Player, _dependency.winningTeam);
			int player = playersOnTeam[0];
			val.GetComponent<VictoryLabelSetter>().SetVictoryLabel(string.Format(StringTableBase<StringTable>.Instance.GetString("strOtherWasVictorius"), playerNamesContainer.GetPlayerName(player)));
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Pit_End_Defeated), 0);
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
