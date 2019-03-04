using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadInvisibilityStatsEngine : SingleEntityViewEngine<LoadInvisibilityStatsNode>, IInitialize
	{
		private IDictionary<int, WeaponStatsData> _weaponStatsData;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			ILoadWeaponStatsRequest loadWeaponStatsRequest = serviceFactory.Create<ILoadWeaponStatsRequest>();
			loadWeaponStatsRequest.SetAnswer(new ServiceAnswer<IDictionary<int, WeaponStatsData>>(delegate(IDictionary<int, WeaponStatsData> statsData)
			{
				_weaponStatsData = statsData;
			})).Execute();
		}

		protected override void Add(LoadInvisibilityStatsNode obj)
		{
			int key = ItemDescriptorKey.GenerateKey(ItemCategory.GhostModule, ItemSize.T5);
			WeaponStatsData weaponStats = _weaponStatsData[key];
			SetInvisibilityStats(obj, weaponStats);
		}

		protected override void Remove(LoadInvisibilityStatsNode obj)
		{
		}

		private void SetInvisibilityStats(LoadInvisibilityStatsNode node, WeaponStatsData weaponStats)
		{
			node.cloakStatsComponent.toInvisibleDuration = weaponStats.toInvisibleDuration;
			node.cloakStatsComponent.toVisibleDuration = weaponStats.toVisibleDuration;
		}
	}
}
