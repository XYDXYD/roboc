using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;
using System;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Mothership
{
	internal class SinglePlayerCampaignImplementor : MonoBehaviour, IWidgetCounterComponent
	{
		[SerializeField]
		private UILabel campaignNameUILabel;

		[SerializeField]
		private UITexture campaignImageUITexture;

		[SerializeField]
		private UISprite[] rankingUiSprites;

		[SerializeField]
		private UIButton uiButton;

		[SerializeField]
		private UILabel campaignTypeUILabel;

		public DispatchOnChange<Campaign?> campaignClicked
		{
			get;
			set;
		}

		public DispatchOnChange<Campaign?> campaignSet
		{
			get;
			set;
		}

		public Texture2D imageTexture
		{
			set
			{
				campaignImageUITexture.set_mainTexture(value);
			}
		}

		public CampaignType campaignType
		{
			set
			{
				LocalizeCampaignType(value);
			}
		}

		public int WidgetCounterMaxValue
		{
			get;
			set;
		}

		public int WidgetCounterValue
		{
			set
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				rankingUiSprites[value].set_color(Color.get_white());
			}
		}

		public SinglePlayerCampaignImplementor()
			: this()
		{
			campaignSet = null;
			campaignClicked = null;
		}

		public void Initialise()
		{
			int instanceID = this.get_gameObject().GetInstanceID();
			campaignClicked = new DispatchOnChange<Campaign?>(instanceID);
			campaignSet = new DispatchOnChange<Campaign?>(instanceID);
			campaignSet.NotifyOnValueSet((Action<int, Campaign?>)OnCampaignSet);
		}

		private unsafe void Start()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnDestroy()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void ReLocalize()
		{
			if (campaignSet.get_value().HasValue)
			{
				ShowCampaign(campaignSet.get_value().Value);
			}
		}

		private unsafe void OnCampaignSet(int entityId, Campaign? campaign)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			this.get_gameObject().SetActive(campaign.HasValue);
			if (campaign.HasValue)
			{
				ShowCampaign(campaign.Value);
				uiButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			}
			else
			{
				uiButton.onClick.Remove(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			}
		}

		private void ShowCampaign(Campaign campaign)
		{
			campaignNameUILabel.set_text(StringTableBase<StringTable>.Instance.GetString(campaign.CampaignName));
			LocalizeCampaignType(campaign.campaignType);
		}

		private void LocalizeCampaignType(CampaignType type)
		{
			switch (type)
			{
			case CampaignType.TimedElimination:
				campaignTypeUILabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCampaignTypeTimedElimination"));
				break;
			case CampaignType.Survival:
				campaignTypeUILabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCampaignTypeSurvival"));
				break;
			case CampaignType.Elimination:
				campaignTypeUILabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCampaignTypeElimination"));
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
		}

		private void Clicked()
		{
			campaignClicked.set_value(campaignSet.get_value());
			campaignClicked.set_value((Campaign?)null);
		}
	}
}
