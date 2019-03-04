using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Simulation.Implementors
{
	internal class SurvivalImplementor : MonoBehaviour, ISurvivalComponent
	{
		[SerializeField]
		private UILabel timeLeftLabel;

		public string timeLeft
		{
			set
			{
				timeLeftLabel.set_text(value);
			}
		}

		public bool isActive
		{
			set
			{
				this.get_gameObject().SetActive(value);
			}
		}

		public SurvivalImplementor()
			: this()
		{
		}
	}
}
