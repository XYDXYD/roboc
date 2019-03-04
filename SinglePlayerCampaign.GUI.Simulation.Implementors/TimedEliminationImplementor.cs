using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Simulation.Implementors
{
	internal class TimedEliminationImplementor : MonoBehaviour, ITimedEliminationComponent
	{
		[SerializeField]
		private UILabel remainingEnemiesLabel;

		[SerializeField]
		private UILabel timeLeftLabel;

		public int remainingEnemies
		{
			set
			{
				remainingEnemiesLabel.set_text(value.ToString());
			}
		}

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

		public TimedEliminationImplementor()
			: this()
		{
		}
	}
}
