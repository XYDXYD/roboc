using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeamDeathmatchStatsView : MonoBehaviour, IInitialize
	{
		public UISlider ownSlider;

		public UILabel ownTeamScore;

		public GameObject ownThumb;

		public UISlider enemySlider;

		public UILabel enemyTeamScore;

		public GameObject enemyThumb;

		public UILabel timeOutLabel;

		private int _limit = 50;

		private int _ownKills;

		private int _enemyKills;

		[Inject]
		internal TeamDeathMatchStatsPresenter presenter
		{
			private get;
			set;
		}

		public TeamDeathmatchStatsView()
			: this()
		{
		}

		private void Start()
		{
			ownSlider.set_enabled(false);
			ownThumb.SetActive(false);
			ownTeamScore.set_text($"0/{_limit}");
			enemySlider.set_enabled(false);
			enemyThumb.SetActive(false);
			enemyTeamScore.set_text($"0/{_limit}");
			timeOutLabel.set_enabled(false);
		}

		void IInitialize.OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void SetKillLimit(int limit)
		{
			_limit = limit;
			ownTeamScore.set_text($"{_ownKills}/{_limit}");
			enemyTeamScore.set_text($"{_enemyKills}/{_limit}");
		}

		public void UpdateStats(bool isOwnTeam, int kills)
		{
			UISlider val = (!isOwnTeam) ? enemySlider : ownSlider;
			if (((!isOwnTeam) ? _enemyKills : _ownKills) == 0)
			{
				val.set_enabled(true);
			}
			val.set_value((float)kills / (float)_limit);
			if (isOwnTeam)
			{
				_ownKills = kills;
				ownTeamScore.set_text($"{kills}/{_limit}");
			}
			else
			{
				_enemyKills = kills;
				enemyTeamScore.set_text($"{kills}/{_limit}");
			}
			val.ForceUpdate();
		}

		public void SetNextTeamWins()
		{
			timeOutLabel.set_enabled(true);
		}
	}
}
