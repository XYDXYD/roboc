using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware
{
	internal sealed class LoadDiscShieldStatsEngine : SingleEntityViewEngine<DiscShieldSettingsNode>, IInitialize
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

		protected override void Add(DiscShieldSettingsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[ItemDescriptorKey.GenerateKey(ItemCategory.ShieldModule, ItemSize.T5)];
			SetModuleStats(obj, weaponStats);
		}

		protected override void Remove(DiscShieldSettingsNode obj)
		{
		}

		private void SetModuleStats(DiscShieldSettingsNode node, WeaponStatsData weaponStats)
		{
			node.settingsComponent.discShieldLifeTime = weaponStats.shieldLifetime;
		}
	}
}
