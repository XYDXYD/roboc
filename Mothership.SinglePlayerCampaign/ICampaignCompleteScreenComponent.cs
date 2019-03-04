using UnityEngine;

namespace Mothership.SinglePlayerCampaign
{
	internal interface ICampaignCompleteScreenComponent
	{
		GameObject screenGameObject
		{
			get;
		}

		Animation screenAnimation
		{
			get;
		}

		UILabel campaignLabel
		{
			get;
		}

		UILabel difficultyLabel
		{
			get;
		}
	}
}
