using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadEmpModuleStatsEngine : SingleEntityViewEngine<LoadEmpModuleStatsNode>, IInitialize
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

		protected override void Add(LoadEmpModuleStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetModuleStats(obj, weaponStats);
		}

		protected override void Remove(LoadEmpModuleStatsNode obj)
		{
		}

		private void SetModuleStats(LoadEmpModuleStatsNode node, WeaponStatsData weaponStats)
		{
			node.countdownComponent.countdown = weaponStats.countdownTime;
			node.stunDurationComponent.stunDuration = weaponStats.stunTime;
			node.stunRadiusComponent.stunRadius = weaponStats.stunRadius;
		}
	}
}
