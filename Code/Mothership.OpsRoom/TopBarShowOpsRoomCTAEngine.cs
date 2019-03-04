using Svelto.DataStructures;
using Svelto.ECS;
using System;

namespace Mothership.OpsRoom
{
	internal class TopBarShowOpsRoomCTAEngine : MultiEntityViewsEngine<OpsRoomShowOpsRoomCTAEntityView, TopBarShowOpsRoomCTAEntityView>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(OpsRoomShowOpsRoomCTAEntityView entityView)
		{
			entityView.opsRoomCTAValuesComponent.unspentTP.NotifyOnValueSet((Action<int, int>)OnValuesChanged);
			entityView.opsRoomCTAValuesComponent.newQuests.NotifyOnValueSet((Action<int, int>)OnValuesChanged);
		}

		protected override void Remove(OpsRoomShowOpsRoomCTAEntityView entityView)
		{
			entityView.opsRoomCTAValuesComponent.unspentTP.StopNotify((Action<int, int>)OnValuesChanged);
			entityView.opsRoomCTAValuesComponent.newQuests.StopNotify((Action<int, int>)OnValuesChanged);
		}

		protected override void Add(TopBarShowOpsRoomCTAEntityView entityView)
		{
			UpdateLabels();
		}

		protected override void Remove(TopBarShowOpsRoomCTAEntityView entityView)
		{
		}

		private void OnValuesChanged(int entityId, int value)
		{
			UpdateLabels();
		}

		private void UpdateLabels()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<OpsRoomShowOpsRoomCTAEntityView> val = entityViewsDB.QueryEntityViews<OpsRoomShowOpsRoomCTAEntityView>();
			if (val.get_Count() != 0)
			{
				OpsRoomShowOpsRoomCTAEntityView opsRoomShowOpsRoomCTAEntityView = val.get_Item(0);
				int num = opsRoomShowOpsRoomCTAEntityView.opsRoomCTAValuesComponent.unspentTP.get_value() + opsRoomShowOpsRoomCTAEntityView.opsRoomCTAValuesComponent.newQuests.get_value();
				FasterListEnumerator<TopBarShowOpsRoomCTAEntityView> enumerator = entityViewsDB.QueryEntityViews<TopBarShowOpsRoomCTAEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						TopBarShowOpsRoomCTAEntityView current = enumerator.get_Current();
						current.opsRoomCtaComponent.gameObject.SetActive(num > 0);
						current.opsRoomCtaComponent.label.set_text(num.ToString());
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
