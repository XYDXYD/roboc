using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadModuleStatsEngine : SingleEntityViewEngine<LoadModuleStatsNode>, IInitialize
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

		protected override void Add(LoadModuleStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetModuleStats(obj, weaponStats);
		}

		protected override void Remove(LoadModuleStatsNode obj)
		{
		}

		private void SetModuleStats(LoadModuleStatsNode node, WeaponStatsData weaponStats)
		{
			node.cooldownComponent.weaponCooldown = weaponStats.cooldownBetweenShots;
			node.manaComponent.weaponFireCost = weaponStats.manaCost;
			node.rangeComponent.moduleRange = weaponStats.moduleRange;
		}
	}
}
