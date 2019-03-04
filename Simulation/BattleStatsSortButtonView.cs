using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Simulation
{
	internal class BattleStatsSortButtonView : MonoBehaviour, IChainListener, IInitialize
	{
		public enum SortingType
		{
			Team,
			Stats
		}

		public GameObject arrow;

		public UIButton targetButton;

		public InGameStatId statId;

		public SortingType sortingType;

		public Color normalColour;

		public Color highlightColour;

		private bool _sortAscending;

		[Inject]
		internal BattleStatsPresenter battleStatsPresenter
		{
			get;
			private set;
		}

		public BattleStatsSortButtonView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			battleStatsPresenter.AddSortButton(this);
		}

		public void ResetSorting()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			_sortAscending = true;
			arrow.SetActive(false);
			SetButtonColor(normalColour);
		}

		public void SetButtonColor(Color colour)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			UIButton[] componentsInChildren = this.GetComponentsInChildren<UIButton>();
			foreach (UIButton val in componentsInChildren)
			{
				if (val.tweenTarget.get_gameObject() == targetButton.get_gameObject())
				{
					val.set_defaultColor(colour);
				}
			}
		}

		public void Listen(object message)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			if (!(message is ButtonType))
			{
				return;
			}
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.Confirm)
			{
				battleStatsPresenter.ResetSortButtons(this);
				_sortAscending = !_sortAscending;
				arrow.get_transform().set_localRotation(Quaternion.Euler(new Vector3(0f, 0f, (float)(_sortAscending ? 180 : 0))));
				arrow.SetActive(true);
				SetButtonColor(highlightColour);
				switch (sortingType)
				{
				default:
					return;
				case SortingType.Team:
					battleStatsPresenter.SortPlayerListByTeam(_sortAscending);
					break;
				case SortingType.Stats:
					battleStatsPresenter.SortPlayerListByStat(statId, _sortAscending);
					break;
				}
				battleStatsPresenter.UpdatePlayerWidgets();
			}
		}
	}
}
