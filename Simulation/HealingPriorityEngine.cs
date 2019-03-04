using Simulation.Hardware;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class HealingPriorityEngine : IInitialize, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private IDictionary<int, WeaponStatsData> _weaponStatsData;

		private HealingAppliedObserver _healingAppliedObserver;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe HealingPriorityEngine(HealingAppliedObserver healingAppliedObserver)
		{
			_healingAppliedObserver = healingAppliedObserver;
			_healingAppliedObserver.AddAction(new ObserverAction<HealingAppliedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		void IInitialize.OnDependenciesInjected()
		{
			ILoadWeaponStatsRequest loadWeaponStatsRequest = serviceFactory.Create<ILoadWeaponStatsRequest>();
			loadWeaponStatsRequest.SetAnswer(new ServiceAnswer<IDictionary<int, WeaponStatsData>>(delegate(IDictionary<int, WeaponStatsData> statsData)
			{
				_weaponStatsData = statsData;
			})).Execute();
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_healingAppliedObserver.RemoveAction(new ObserverAction<HealingAppliedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnHealingApplied(ref HealingAppliedData parameter)
		{
			HealingPriorityNode healingPriorityNode = null;
			if (entityViewsDB.TryQueryEntityView<HealingPriorityNode>(parameter.targetId, ref healingPriorityNode))
			{
				if (healingPriorityNode.healingPriorityComponent.healerId == -1)
				{
					healingPriorityNode.healingPriorityComponent.healerId = parameter.healerId;
				}
				if (healingPriorityNode.healingPriorityComponent.healerId == parameter.healerId)
				{
					float nanoHealingPriorityTime = _weaponStatsData[parameter.weaponKey].nanoHealingPriorityTime;
					healingPriorityNode.healingPriorityComponent.priorityExpireTime = Time.get_timeSinceLevelLoad() + nanoHealingPriorityTime;
				}
			}
		}

		public void Ready()
		{
			TaskRunner.get_Instance().AllocateNewTaskRoutine().SetScheduler(StandardSchedulers.get_coroutineScheduler())
				.SetEnumerator(Update())
				.Start((Action<PausableTaskException>)null, (Action)null);
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<HealingPriorityNode> machineNodes = entityViewsDB.QueryEntityViews<HealingPriorityNode>();
				for (int i = 0; i < machineNodes.get_Count(); i++)
				{
					HealingPriorityNode healingPriorityNode = machineNodes.get_Item(i);
					IHealingPriorityComponent healingPriorityComponent = healingPriorityNode.healingPriorityComponent;
					if (healingPriorityComponent.healerId != -1 && Time.get_timeSinceLevelLoad() > healingPriorityComponent.priorityExpireTime)
					{
						healingPriorityComponent.healerId = -1;
					}
				}
				yield return null;
			}
		}
	}
}
