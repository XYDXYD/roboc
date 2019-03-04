using Battle;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class TeamDeathMatchStatsPresenter : IInitialize, IHudElement, IWaitForFrameworkDestruction
	{
		private TeamDeathmatchStatsView _view;

		private uint _userTeam;

		private int _killLimit = -1;

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
		public IServiceRequestFactory serviceFactory
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

		[Inject]
		public TDMMusicManager musicManager
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_userTeam = battlePlayers.MyTeam;
		}

		public void OnFrameworkDestroyed()
		{
			battleHudStyleController.RemoveHud(this);
		}

		public void SetStyle(HudStyle style)
		{
			if (style == HudStyle.HideAllButChat)
			{
				_view.Hide();
			}
		}

		public void OnSetKillLimit(int killLimit)
		{
			_killLimit = killLimit;
			if (_view != null)
			{
				_view.SetKillLimit(_killLimit);
			}
			musicManager.SetKillLimit(killLimit);
		}

		private void OnSetKillLimitFailure(ServiceBehaviour behaviour)
		{
			RemoteLogger.Error(behaviour.errorTitle, behaviour.mainText, null);
			_killLimit = 25;
			if (_view != null)
			{
				_view.SetKillLimit(_killLimit);
			}
		}

		public void SetView(TeamDeathmatchStatsView view)
		{
			_view = view;
			if (_killLimit > -1)
			{
				_view.SetKillLimit(_killLimit);
			}
			battleHudStyleController.AddHud(this);
		}

		public void UpdateScore(IDictionary<int, int> scores, bool hasTimeExpired)
		{
			IEnumerator<KeyValuePair<int, int>> enumerator = scores.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value > 0)
				{
					_view.UpdateStats(enumerator.Current.Key == _userTeam, enumerator.Current.Value);
					musicManager.UpdateKills(enumerator.Current.Value);
				}
			}
			if (hasTimeExpired)
			{
				_view.SetNextTeamWins();
			}
		}
	}
}
