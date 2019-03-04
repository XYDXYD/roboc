using Mothership.DailyQuest;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using System;
using System.Collections.Generic;

namespace Mothership.OpsRoom
{
	internal class OpsRoomShowQuestsCTAEngine : SingleEntityViewEngine<OpsRoomShowQuestsCTAEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly DailyQuestController _dailyQuestController;

		private readonly DailyQuestsObserver _dailyQuestsObserver;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public OpsRoomShowQuestsCTAEngine(DailyQuestController dailyQuestController, DailyQuestsObserver dailyQuestsObserver)
		{
			_dailyQuestController = dailyQuestController;
			_dailyQuestsObserver = dailyQuestsObserver;
		}

		public void Ready()
		{
		}

		protected unsafe override void Add(OpsRoomShowQuestsCTAEntityView entityView)
		{
			if (_dailyQuestController.playerQuestData != null)
			{
				UpdateLabel(_dailyQuestController.playerQuestData);
			}
			_dailyQuestsObserver.AddAction(new ObserverAction<PlayerDailyQuestsData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected unsafe override void Remove(OpsRoomShowQuestsCTAEntityView entityView)
		{
			_dailyQuestsObserver.RemoveAction(new ObserverAction<PlayerDailyQuestsData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void UpdateLabel(ref PlayerDailyQuestsData data)
		{
			UpdateLabel(data);
		}

		private void UpdateLabel(PlayerDailyQuestsData data)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			List<Quest> playerQuests = data.playerQuests;
			int num = 0;
			foreach (Quest item in playerQuests)
			{
				if (!item.seen)
				{
					num++;
				}
			}
			FasterListEnumerator<OpsRoomShowQuestsCTAEntityView> enumerator2 = entityViewsDB.QueryEntityViews<OpsRoomShowQuestsCTAEntityView>().GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					OpsRoomShowQuestsCTAEntityView current2 = enumerator2.get_Current();
					current2.questsCTAComponent.gameObject.SetActive(num > 0);
					current2.questsCTAComponent.label.set_text(num.ToString());
					current2.opsRoomCTAValuesComponent.newQuests.set_value(num);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
		}
	}
}
