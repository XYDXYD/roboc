using Svelto.ECS;
using System;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Mothership
{
	internal class SinglePlayerCampaignListScreen : MonoBehaviour, ISinglePlayerCampaignListComponent
	{
		[SerializeField]
		private GameObject campaignTemplateGO;

		[SerializeField]
		private UIGrid uiGrid;

		[SerializeField]
		private SinglePlayerCampaignInfoScreen infoScreen;

		UIGrid ISinglePlayerCampaignListComponent.uiGrid
		{
			get
			{
				return uiGrid;
			}
		}

		GameObject ISinglePlayerCampaignListComponent.campaignTemplateGO
		{
			get
			{
				return campaignTemplateGO;
			}
		}

		SinglePlayerCampaignInfoScreen ISinglePlayerCampaignListComponent.infoScreen
		{
			get
			{
				return infoScreen;
			}
		}

		public DispatchOnChange<bool> show
		{
			get;
			set;
		}

		public SinglePlayerCampaignListScreen()
			: this()
		{
		}

		public void Initialise()
		{
			show = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			show.NotifyOnValueSet((Action<int, bool>)delegate(int _, bool v)
			{
				this.get_gameObject().SetActive(v);
			});
		}
	}
}
