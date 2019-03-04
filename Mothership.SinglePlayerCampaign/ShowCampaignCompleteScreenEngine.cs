using Simulation;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ES.Legacy;

namespace Mothership.SinglePlayerCampaign
{
	internal class ShowCampaignCompleteScreenEngine : IQueryingEntityViewEngine, IGUIDisplay, IEngine, IComponent
	{
		public GuiScreens screenType => GuiScreens.CampaignComplete;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public bool isScreenBlurred => false;

		public bool hasBackground => true;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void EnableBackground(bool enable)
		{
		}

		public void Ready()
		{
		}

		public GUIShowResult Show()
		{
			LastCompletedCampaignEntityView lastCompletedCampaignEntityView = entityViewsDB.QueryEntityView<LastCompletedCampaignEntityView>(206);
			ILastCompletedCampaignComponent lastCompletedCampaignComponent = lastCompletedCampaignEntityView.lastCompletedCampaignComponent;
			CampaignDataEntityView campaignDataEntityView = entityViewsDB.QueryEntityView<CampaignDataEntityView>(205);
			string key = null;
			Campaign[] campaignData = campaignDataEntityView.campaignDataComponent.campaignData;
			for (int i = 0; i < campaignData.Length; i++)
			{
				Campaign campaign = campaignData[i];
				if (campaign.CampaignID == lastCompletedCampaignComponent.campaignId)
				{
					key = campaign.CampaignName;
					break;
				}
			}
			CampaignCompleteScreenEntityView screen = GetScreen();
			ICampaignCompleteScreenComponent campaignCompleteScreen = screen.campaignCompleteScreen;
			campaignCompleteScreen.campaignLabel.set_text(StringTableBase<StringTable>.Instance.GetString(key));
			campaignCompleteScreen.difficultyLabel.set_text(StringTableBase<StringTable>.Instance.GetString(CampaignDifficultyHelper.difficultyNameKeys[lastCompletedCampaignComponent.difficulty]));
			campaignCompleteScreen.screenGameObject.SetActive(true);
			campaignCompleteScreen.screenAnimation.Play(CampaignDifficultyHelper.animationNames[lastCompletedCampaignComponent.difficulty]);
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			CampaignCompleteScreenEntityView screen = GetScreen();
			if (screen == null)
			{
				return false;
			}
			screen.campaignCompleteScreen.screenGameObject.SetActive(false);
			return true;
		}

		public bool IsActive()
		{
			CampaignCompleteScreenEntityView screen = GetScreen();
			return screen != null && screen.campaignCompleteScreen.screenGameObject.get_activeSelf();
		}

		private CampaignCompleteScreenEntityView GetScreen()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CampaignCompleteScreenEntityView> val = entityViewsDB.QueryEntityViews<CampaignCompleteScreenEntityView>();
			return (val.get_Count() != 0) ? val.get_Item(0) : null;
		}
	}
}
