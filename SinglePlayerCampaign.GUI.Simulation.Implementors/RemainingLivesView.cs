using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Simulation.Implementors
{
	internal class RemainingLivesView : MonoBehaviour, IWidgetCounterComponent
	{
		[SerializeField]
		private UILabel livesRemaining;

		public int WidgetCounterMaxValue
		{
			get;
			set;
		}

		public int WidgetCounterValue
		{
			set
			{
				livesRemaining.set_text(value.ToString());
			}
		}

		public RemainingLivesView()
			: this()
		{
		}
	}
}
