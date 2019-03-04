using SinglePlayerCampaign.GUI.Simulation.EntityViews;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using System;

namespace SinglePlayerCampaign.GUI.Simulation.Engines
{
	internal class UpdatePlayerLivesWidgetsEngine : MultiEntityViewsEngine<CampaignShowRemainingLivesEntityView, RemainingLivesWidgetEntityView>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignShowRemainingLivesEntityView entityView)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			entityView.remainingLivesComponent.remainingLives.NotifyOnValueSet((Action<int, int>)UpdatePlayerLivesWidgets);
			FasterListEnumerator<RemainingLivesWidgetEntityView> enumerator = entityViewsDB.QueryEntityViews<RemainingLivesWidgetEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RemainingLivesWidgetEntityView current = enumerator.get_Current();
					InitialiseWidget(entityView, current);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		protected override void Remove(CampaignShowRemainingLivesEntityView entityView)
		{
			entityView.remainingLivesComponent.remainingLives.StopNotify((Action<int, int>)UpdatePlayerLivesWidgets);
		}

		protected override void Add(RemainingLivesWidgetEntityView entityView)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CampaignShowRemainingLivesEntityView> val = entityViewsDB.QueryEntityViews<CampaignShowRemainingLivesEntityView>();
			if (val.get_Count() != 0)
			{
				InitialiseWidget(val.get_Item(0), entityView);
			}
		}

		protected override void Remove(RemainingLivesWidgetEntityView entityView)
		{
		}

		private static void InitialiseWidget(CampaignShowRemainingLivesEntityView campaignShowRemainingLivesEntityView, RemainingLivesWidgetEntityView remainingLivesWidgetEntityView)
		{
			remainingLivesWidgetEntityView.RemainingPlayerLivesWidgetCounterComponent.WidgetCounterMaxValue = campaignShowRemainingLivesEntityView.remainingLivesComponent.remainingLives.get_value();
			remainingLivesWidgetEntityView.RemainingPlayerLivesWidgetCounterComponent.WidgetCounterValue = campaignShowRemainingLivesEntityView.remainingLivesComponent.remainingLives.get_value();
		}

		private void UpdatePlayerLivesWidgets(int entityID, int remainingPlayerLivesCount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<RemainingLivesWidgetEntityView> enumerator = entityViewsDB.QueryEntityViews<RemainingLivesWidgetEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RemainingLivesWidgetEntityView current = enumerator.get_Current();
					current.RemainingPlayerLivesWidgetCounterComponent.WidgetCounterValue = remainingPlayerLivesCount;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}
}
