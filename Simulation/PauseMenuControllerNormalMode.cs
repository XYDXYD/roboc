using Simulation.Hardware.Weapons;

namespace Simulation
{
	internal sealed class PauseMenuControllerNormalMode : PauseMenuControllerMultiPlayer
	{
		public PauseMenuControllerNormalMode(BattleLeftEventObservable observable, PlayerQuitRequestCompleteObserver quitRequestCompleteObserver)
			: base(observable, quitRequestCompleteObserver)
		{
		}

		public override void ActivateCorrectCoolDown()
		{
			if (WorldSwitching.IsRanked())
			{
				_pauseMenu.SurrenderCoolDownNormalRankedModeObject.SetActive(true);
			}
			else
			{
				_pauseMenu.SurrenderCoolDownNormalModeObject.SetActive(true);
			}
		}

		public override GUIShowResult Show()
		{
			if (base.LobbyGameStartPresenter.hasBeenClosed && base.livePlayersContainer.IsPlayerAlive(TargetType.Player, base.playerTeamsContainer.localPlayerId))
			{
				if (WorldSwitching.IsRanked())
				{
					_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.RankedNormalMode);
				}
				else
				{
					_pauseMenu.Show(PauseMenuWidgetSimulation.MenuType.NormalMode);
				}
				return GUIShowResult.Showed;
			}
			return GUIShowResult.NotShowed;
		}
	}
}
