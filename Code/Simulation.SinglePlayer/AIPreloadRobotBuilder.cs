using Services.Simulation;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	internal class AIPreloadRobotBuilder : IInitialize
	{
		private List<int> _defaultWeaponOrderSubcategories;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			IGetDefaultWeaponOrderSubcategoriesRequest getDefaultWeaponOrderSubcategoriesRequest = serviceFactory.Create<IGetDefaultWeaponOrderSubcategoriesRequest>();
			getDefaultWeaponOrderSubcategoriesRequest.SetAnswer(new ServiceAnswer<List<int>>(SetDefaultWeaponOrderSubcategories));
			getDefaultWeaponOrderSubcategoriesRequest.Execute();
		}

		private void SetDefaultWeaponOrderSubcategories(List<int> defaultWeaponOrderSubcategories)
		{
			_defaultWeaponOrderSubcategories = defaultWeaponOrderSubcategories;
		}

		public void GenerateWeaponOrder(PreloadedMachine preloadedMachine)
		{
			HashSet<int> hashSet = new HashSet<int>();
			FasterList<InstantiatedCube> allInstantiatedCubes = preloadedMachine.machineMap.GetAllInstantiatedCubes();
			InstantiatedCube[] array = allInstantiatedCubes.ToArrayFast();
			for (int num = allInstantiatedCubes.get_Count() - 1; num >= 0; num--)
			{
				InstantiatedCube instantiatedCube = array[num];
				ItemDescriptor itemDescriptor = instantiatedCube.persistentCubeData.itemDescriptor;
				if (itemDescriptor != null && itemDescriptor.isActivable)
				{
					hashSet.Add(itemDescriptor.GenerateKey());
				}
			}
			if (hashSet.Count <= 0)
			{
				return;
			}
			List<int> list = new List<int>();
			for (int i = 0; i < _defaultWeaponOrderSubcategories.Count; i++)
			{
				int item = _defaultWeaponOrderSubcategories[i];
				if (hashSet.Contains(_defaultWeaponOrderSubcategories[i]))
				{
					list.Add(item);
				}
			}
			preloadedMachine.weaponOrder = new WeaponOrderSimulation(list.ToArray());
		}
	}
}
