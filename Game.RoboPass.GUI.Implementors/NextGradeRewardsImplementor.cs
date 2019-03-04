using Game.RoboPass.GUI.Components;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	public class NextGradeRewardsImplementor : MonoBehaviour, IBattleSummaryNextGradeRewardsComponent
	{
		[SerializeField]
		private UIGrid _nextRewardsGrid;

		[SerializeField]
		private GameObject _freeRewardItem;

		[SerializeField]
		private GameObject _deluxeRewardItem;

		[SerializeField]
		private GameObject _nextGradeRewardsLabelContainer;

		public UIGrid NextRewardsGrid => _nextRewardsGrid;

		public GameObject FreeRewardItem => _freeRewardItem;

		public GameObject DeluxeRewardItem => _deluxeRewardItem;

		public bool FreeRewardItemActive
		{
			set
			{
				_freeRewardItem.SetActive(value);
			}
		}

		public bool DeluxeRewardItemActive
		{
			set
			{
				_deluxeRewardItem.SetActive(value);
			}
		}

		public bool NextGradeRewardsLabelActive
		{
			set
			{
				_nextGradeRewardsLabelContainer.SetActive(value);
			}
		}

		public NextGradeRewardsImplementor()
			: this()
		{
		}
	}
}
