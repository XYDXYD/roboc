using Simulation.Hardware.Weapons.Chaingun;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal sealed class LoadChaingunWeaponStatsEngine : SingleEntityViewEngine<LoadChaingunStatsNode>, IInitialize
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

		protected override void Add(LoadChaingunStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetChaingunStats(obj, weaponStats);
		}

		protected override void Remove(LoadChaingunStatsNode obj)
		{
		}

		private void SetChaingunStats(LoadChaingunStatsNode weaponStatsNode, WeaponStatsData weaponStats)
		{
			PopulateSpinUpStats(weaponStatsNode.spinUpComponent, weaponStats);
		}

		private void PopulateSpinUpStats(IWeaponSpinStatsComponent stats, WeaponStatsData weaponStats)
		{
			stats.spinUpTime = weaponStats.spinUpTime;
			stats.spinDownTime = weaponStats.spinDownTime;
			stats.spinInitialCooldown = weaponStats.spinInitialCooldown;
		}
	}
}
