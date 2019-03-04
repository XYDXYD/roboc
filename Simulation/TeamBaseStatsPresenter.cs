using Battle;
using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class TeamBaseStatsPresenter : IInitialize, IHudElement, IWaitForFrameworkDestruction
	{
		private TeamBaseStatsView _view;

		[Inject]
		public TeamBaseProgressDispatcher teamBaseProgressDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		public IHudStyleController battleHudStyleController
		{
			private get;
			set;
		}

		public void OnFrameworkDestroyed()
		{
			battleHudStyleController.RemoveHud(this);
		}

		void IInitialize.OnDependenciesInjected()
		{
			uint myTeam = battlePlayers.MyTeam;
			HashSet<int> allTeams = battlePlayers.GetAllTeams();
			foreach (int item in allTeams)
			{
				if (item == myTeam)
				{
					teamBaseProgressDispatcher.RegisterBaseChange(item, OnMyTeamStatsChanged);
				}
				else
				{
					teamBaseProgressDispatcher.RegisterBaseChange(item, OnEnemyTeamStatsChanged);
				}
			}
		}

		public void SetStyle(HudStyle style)
		{
			if (style == HudStyle.HideAllButChat)
			{
				_view.Hide();
			}
		}

		public void SetView(TeamBaseStatsView view)
		{
			_view = view;
			battleHudStyleController.AddHud(this);
		}

		public void OnMyTeamStatsChanged(float teamProgress)
		{
			_view.UpdateStats(isMyTeam: true, teamProgress);
		}

		public void OnEnemyTeamStatsChanged(float teamProgress)
		{
			_view.UpdateStats(isMyTeam: false, teamProgress);
		}
	}
}
