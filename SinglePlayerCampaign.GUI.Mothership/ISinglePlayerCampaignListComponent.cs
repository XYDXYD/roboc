using Svelto.ECS;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Mothership
{
	internal interface ISinglePlayerCampaignListComponent
	{
		SinglePlayerCampaignInfoScreen infoScreen
		{
			get;
		}

		GameObject campaignTemplateGO
		{
			get;
		}

		UIGrid uiGrid
		{
			get;
		}

		DispatchOnChange<bool> show
		{
			get;
			set;
		}
	}
}
