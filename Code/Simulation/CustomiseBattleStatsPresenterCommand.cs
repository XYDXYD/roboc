using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal class CustomiseBattleStatsPresenterCommand : ICommand
	{
		protected GameModeType GameMode;

		[Inject]
		internal BattleStatsPresenter battleStatPresenter
		{
			private get;
			set;
		}

		public ICommand Inject(GameModeType gameMode_)
		{
			GameMode = gameMode_;
			return this;
		}

		public virtual void Execute()
		{
			switch (GameMode)
			{
			case GameModeType.TestMode:
				break;
			case GameModeType.Normal:
				battleStatPresenter.ConfigureForBattleArenaStyle();
				break;
			case GameModeType.Pit:
				battleStatPresenter.ConfigureForPitModeStyle();
				break;
			case GameModeType.EditMode:
				battleStatPresenter.ConfigureForTDMModeStyle();
				break;
			case GameModeType.PraticeMode:
			case GameModeType.Campaign:
				battleStatPresenter.ConfigureForTDMModeStyle();
				break;
			case GameModeType.SuddenDeath:
				battleStatPresenter.ConfigureForTDMModeStyle();
				break;
			case GameModeType.TeamDeathmatch:
				battleStatPresenter.ConfigureForTDMModeStyle();
				break;
			}
		}
	}
}
