using Game.RoboPass.EntityViews;
using Game.RoboPass.GUI.EntityViews;
using Services.Analytics;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Game.RoboPass.GUI.Engines
{
	internal class RoboPassButtonsOnClickGoToStoreEngine : MultiEntityViewsEngine<RoboPassScreenGetPremiumEntityView, RoboPassGetRoboPassPlusButtonEntityView, RoboPassSeasonDataEntityView>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RoboPassButtonsOnClickGoToStoreEngine(IAnalyticsRequestFactory analyticsRequestFactory, IGUIInputController guiInputController)
		{
		}

		public void Ready()
		{
		}

		protected override void Add(RoboPassScreenGetPremiumEntityView entityView)
		{
			entityView.buttonComponent.buttonPressed.NotifyOnValueSet((Action<int, bool>)GoToStoreScreen);
		}

		protected override void Remove(RoboPassScreenGetPremiumEntityView entityView)
		{
			entityView.buttonComponent.buttonPressed.StopNotify((Action<int, bool>)GoToStoreScreen);
		}

		protected override void Add(RoboPassGetRoboPassPlusButtonEntityView entityView)
		{
			entityView.buttonComponent.buttonPressed.NotifyOnValueSet((Action<int, bool>)GoToStoreScreen);
		}

		protected override void Remove(RoboPassGetRoboPassPlusButtonEntityView entityView)
		{
			entityView.buttonComponent.buttonPressed.StopNotify((Action<int, bool>)GoToStoreScreen);
		}

		protected override void Add(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonInfoComponent.dataUpdated.NotifyOnValueSet((Action<int, bool>)HidePurchaseRobopassButtonIfSeasonHasEnded);
		}

		protected override void Remove(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonInfoComponent.dataUpdated.StopNotify((Action<int, bool>)HidePurchaseRobopassButtonIfSeasonHasEnded);
		}

		private void GoToStoreScreen(int entityId, bool buttonPressed)
		{
			if (buttonPressed)
			{
				Application.OpenURL("https://rc.qq.com/client/r2.htm");
			}
		}

		private void HidePurchaseRobopassButtonIfSeasonHasEnded(int entityId, bool dataUpdated)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			if (dataUpdated)
			{
				RoboPassSeasonDataEntityView roboPassSeasonDataEntityView = entityViewsDB.QueryEntityView<RoboPassSeasonDataEntityView>(entityId);
				if (!roboPassSeasonDataEntityView.roboPassSeasonInfoComponent.isValidSeason)
				{
					FasterListEnumerator<RoboPassGetRoboPassPlusButtonEntityView> enumerator = entityViewsDB.QueryEntityViews<RoboPassGetRoboPassPlusButtonEntityView>().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							RoboPassGetRoboPassPlusButtonEntityView current = enumerator.get_Current();
							current.uiElementVisibleComponent.uiElementHidden = true;
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
			}
		}
	}
}
