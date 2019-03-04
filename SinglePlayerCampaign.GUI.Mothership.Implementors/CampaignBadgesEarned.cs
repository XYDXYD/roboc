using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Mothership.Implementors
{
	internal class CampaignBadgesEarned : MonoBehaviour, IWidgetCounterComponent
	{
		[SerializeField]
		private GameObject[] badgesEarned;

		public int WidgetCounterMaxValue
		{
			get;
			set;
		}

		public int WidgetCounterValue
		{
			set
			{
				for (int i = 0; i < badgesEarned.Length; i++)
				{
					GameObject val = badgesEarned[i];
					bool active = i < value;
					val.SetActive(active);
				}
			}
		}

		public CampaignBadgesEarned()
			: this()
		{
		}
	}
}
