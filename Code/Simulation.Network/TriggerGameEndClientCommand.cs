using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using UnityEngine;

namespace Simulation.Network
{
	internal sealed class TriggerGameEndClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GameEndDependency _dependency;

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
		public IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		public VOManager voManager
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as GameEndDependency);
			return this;
		}

		public void Execute()
		{
			gameStateClient.ChangeStateToGameEnded(GameStateResult.Draw, _dependency.gameEndReason);
			gameEndedObserver.GameEnded(won: false);
			GameObject val = gameObjectFactory.Build("HUD_DRAW");
			voManager.PlayVO(AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_Draw));
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
