using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Simulation.Implementors
{
	internal class EliminationImplementor : MonoBehaviour, IEliminationComponent
	{
		[SerializeField]
		private UILabel remainingEnemiesLabel;

		public int remainingEnemies
		{
			set
			{
				remainingEnemiesLabel.set_text(value.ToString());
			}
		}

		public bool isActive
		{
			set
			{
				this.get_gameObject().SetActive(value);
			}
		}

		public EliminationImplementor()
			: this()
		{
		}
	}
}
