using Game.ECS.GUI.Components;
using Game.RoboPass.Components;
using Game.RoboPass.EntityViews;
using Game.RoboPass.GUI.Components;
using Game.RoboPass.GUI.EntityViews;
using Services.Requests.Interfaces;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Game.RoboPass.GUI.Engines
{
	internal class RoboPassMothershipScreenDisplayEngine : MultiEntityViewsEngine<RoboPassSeasonScreenEntityView, RoboPassSeasonDataEntityView, RoboPassScreenGetPremiumEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private const string STR_LOC_GRADE = "strRoboPassGrade";

		private readonly IGUIInputController _guiInputController;

		private readonly IServiceRequestFactory _serviceReqFactory;

		private readonly PremiumMembership _premiumMembership;

		private readonly RobopassScreenFactory _robopassScreenFactory;

		private readonly string _strLocalizedGrade;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RoboPassMothershipScreenDisplayEngine(IGUIInputController guiInputController, IServiceRequestFactory serviceReqFactory, PremiumMembership premiumMembership, RobopassScreenFactory robopassScreenFactory)
		{
			_guiInputController = guiInputController;
			_premiumMembership = premiumMembership;
			_serviceReqFactory = serviceReqFactory;
			_robopassScreenFactory = robopassScreenFactory;
			_strLocalizedGrade = StringTableBase<StringTable>.Instance.GetString("strRoboPassGrade").ToUpper() + " ";
		}

		public void Ready()
		{
		}

		public void OnFrameworkDestroyed()
		{
			_premiumMembership.onSubscriptionActivated -= HidePremiumSection;
			_premiumMembership.onSubscriptionExpired -= ShowPremiumSection;
		}

		protected override void Add(RoboPassSeasonScreenEntityView entityView)
		{
			entityView.buttonComponent.buttonPressed.NotifyOnValueSet((Action<int, bool>)GoToGarageScreen);
		}

		protected override void Remove(RoboPassSeasonScreenEntityView entityView)
		{
			entityView.buttonComponent.buttonPressed.StopNotify((Action<int, bool>)GoToGarageScreen);
		}

		protected override void Add(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.dataUpdated.NotifyOnValueSet((Action<int, bool>)RefreshEntireRoboPassUi);
		}

		protected override void Remove(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.dataUpdated.StopNotify((Action<int, bool>)RefreshEntireRoboPassUi);
		}

		protected override void Add(RoboPassScreenGetPremiumEntityView entityView)
		{
			TaskRunner.get_Instance().Run(SetupPremiumVisibility(entityView));
		}

		protected override void Remove(RoboPassScreenGetPremiumEntityView entityView)
		{
		}

		private IEnumerator SetupPremiumVisibility(RoboPassScreenGetPremiumEntityView roboPassScreenGetPremiumEV)
		{
			ILoadPlatformConfigurationRequest platformConfigRequest = _serviceReqFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> platformConfigTask = platformConfigRequest.AsTask();
			yield return platformConfigTask;
			if (platformConfigTask.succeeded)
			{
				while (!_premiumMembership.Loaded())
				{
					yield return null;
				}
				bool canBuyPremium = platformConfigTask.result.BuyPremiumAvailable;
				bool hasPremium = _premiumMembership.hasPremiumForLife || _premiumMembership.hasSubscription;
				bool premiumSectionVisible = canBuyPremium && !hasPremium;
				ChangePremiumSectionVisibility(roboPassScreenGetPremiumEV, premiumSectionVisible);
				if (canBuyPremium)
				{
					_premiumMembership.onSubscriptionActivated += HidePremiumSection;
					_premiumMembership.onSubscriptionExpired += ShowPremiumSection;
				}
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(platformConfigTask.behaviour);
			}
		}

		private void RefreshEntireRoboPassUi(int entityID, bool dataUpdated)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			if (dataUpdated)
			{
				RoboPassSeasonDataEntityView roboPassSeasonDataEntityView = entityViewsDB.QueryEntityView<RoboPassSeasonDataEntityView>(entityID);
				IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonPlayerInfoComponent;
				int currentGradeIndex = roboPassSeasonPlayerInfoComponent.currentGradeIndex;
				float progressInGrade = roboPassSeasonPlayerInfoComponent.progressInGrade;
				IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonInfoComponent;
				int gradesHighestIndex = roboPassSeasonInfoComponent.gradesHighestIndex;
				string robopassSeasonName = roboPassSeasonInfoComponent.robopassSeasonName;
				RoboPassXpGradeUIEntityView roboPassXpGradeUIEntityView = entityViewsDB.QueryEntityViews<RoboPassXpGradeUIEntityView>().get_Item(0);
				IProgressBarUIComponent progressBarUIComponent = roboPassXpGradeUIEntityView.progressBarUIComponent;
				progressBarUIComponent.progress = progressInGrade;
				ILabelUIComponent labelUIComponent = roboPassXpGradeUIEntityView.labelUIComponent;
				labelUIComponent.label = _strLocalizedGrade + (currentGradeIndex + 1);
				FasterListEnumerator<RoboPassSeasonTitleUIEntityView> enumerator = entityViewsDB.QueryEntityViews<RoboPassSeasonTitleUIEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						RoboPassSeasonTitleUIEntityView current = enumerator.get_Current();
						current.labelUIComponent.label = robopassSeasonName;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				RoboPassSeasonScreenEntityView roboPassSeasonScreenEntityView = entityViewsDB.QueryEntityViews<RoboPassSeasonScreenEntityView>().get_Item(0);
				IRewardsGridsComponent rewardsGridsComponent = roboPassSeasonScreenEntityView.rewardsGridsComponent;
				rewardsGridsComponent.deluxeRewardsUnlocked = roboPassSeasonDataEntityView.roboPassSeasonPlayerInfoComponent.hasDeluxe;
				if (entityViewsDB.QueryEntityViews<RoboPassScreenGetPremiumEntityView>().get_Count() == 0)
				{
					_robopassScreenFactory.BuildButtonsUI(roboPassSeasonScreenEntityView);
				}
			}
		}

		private void GoToGarageScreen(int entityId, bool buttonPressed)
		{
			if (buttonPressed)
			{
				_guiInputController.CloseCurrentScreen();
			}
		}

		private void ShowPremiumSection()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<RoboPassScreenGetPremiumEntityView> enumerator = entityViewsDB.QueryEntityViews<RoboPassScreenGetPremiumEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RoboPassScreenGetPremiumEntityView current = enumerator.get_Current();
					ChangePremiumSectionVisibility(current, visible: true);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private void HidePremiumSection(TimeSpan timeSpan)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<RoboPassScreenGetPremiumEntityView> enumerator = entityViewsDB.QueryEntityViews<RoboPassScreenGetPremiumEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RoboPassScreenGetPremiumEntityView current = enumerator.get_Current();
					ChangePremiumSectionVisibility(current, visible: false);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private static void ChangePremiumSectionVisibility(RoboPassScreenGetPremiumEntityView roboPassScreenGetPremiumEV, bool visible)
		{
			roboPassScreenGetPremiumEV.uiElementVisibleComponent.uiElementHidden = !visible;
		}
	}
}
