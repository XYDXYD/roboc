using Achievements;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership.Achievements
{
	internal class AchievementCRFMasteryTracker : IInitialize
	{
		[Inject]
		private IAchievementManager achievementManager
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadAllGarages);
		}

		private IEnumerator LoadAllGarages()
		{
			ILoadGarageDataRequest request = serviceFactory.Create<ILoadGarageDataRequest>();
			TaskService<LoadGarageDataRequestResponse> task = new TaskService<LoadGarageDataRequestResponse>(request);
			yield return task;
			if (!task.succeeded)
			{
				yield break;
			}
			FasterList<GarageSlotDependency> garages = task.result.garageSlots;
			int num = 0;
			while (true)
			{
				if (num < garages.get_Count())
				{
					GarageSlotDependency garageSlotDependency = garages.get_Item(num);
					if (garageSlotDependency.crfId != 0 && garageSlotDependency.masteryLevel >= 11)
					{
						break;
					}
					num++;
					continue;
				}
				yield break;
			}
			achievementManager.ReachedMastery10OnCRFBot();
		}
	}
}
