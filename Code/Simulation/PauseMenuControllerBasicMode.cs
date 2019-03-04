using Simulation.Hardware.Weapons;

namespace Simulation
{
	internal sealed class PauseMenuControllerBasicMode : PauseMenuControllerMultiPlayer
	{
		public PauseMenuControllerBasicMode(BattleLeftEventObservable observable, PlayerQuitRequestCompleteObserver quitRequestCompleteObserver)
			: base(observable, quitRequestCompleteObserver)
		{
		}

		public override void ActivateCorrectCoolDown()
		{
			_pauseMenu.SurrenderCoolDownBasicModeObject.SetActive(true);
		}

		public override GUIShowResult Show()
		{
			if (base.LobbyGameStartPresenter.hasBeenClosed && base.livePlayersContainer.IsPlayerAlive(TargetType.Player, base.playerTeamsContainer.localPlayerId))
			{
				_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.BasicMode);
				return GUIShowResult.Showed;
			}
			return GUIShowResult.NotShowed;
		}
	}
}
