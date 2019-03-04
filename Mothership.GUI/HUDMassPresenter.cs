using Svelto.Context;
using Svelto.IoC;

namespace Mothership.GUI
{
	internal class HUDMassPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private HUDMassView _view;

		private int _accumulatedMass;

		[Inject]
		private IMachineMap machineMap
		{
			get;
			set;
		}

		internal void SetView(HUDMassView view)
		{
			_view = view;
			_view.SetValue(0f);
		}

		public void OnDependenciesInjected()
		{
			RegisterEventListeners();
		}

		public void OnFrameworkDestroyed()
		{
			UnregisterEventListeners();
		}

		public void ForceDisplayToZero(bool lockToZero)
		{
			if (lockToZero)
			{
				UnregisterEventListeners();
				_view.SetValue(0f);
			}
			else
			{
				RegisterEventListeners();
			}
		}

		private void RegisterEventListeners()
		{
			machineMap.OnAddCubeAt += CubeLoaded;
			machineMap.OnRemoveCubeAt += CubeUnloaded;
		}

		private void UnregisterEventListeners()
		{
			machineMap.OnAddCubeAt -= CubeLoaded;
			machineMap.OnRemoveCubeAt -= CubeUnloaded;
		}

		private void CubeLoaded(Byte3 arg1, MachineCell cell)
		{
			if (cell.info.persistentCubeData.itemType != ItemType.Cosmetic)
			{
				_accumulatedMass += MachineMassUtility.GetMass(cell.info);
			}
			_view.SetValue(GetDisplayMass());
		}

		private void CubeUnloaded(Byte3 arg1, MachineCell cell)
		{
			if (cell.info.persistentCubeData.itemType != ItemType.Cosmetic)
			{
				_accumulatedMass -= MachineMassUtility.GetMass(cell.info);
			}
			_view.SetValue(GetDisplayMass());
		}

		private float GetDisplayMass()
		{
			return MachineMassUtility.GetDisplayMass(_accumulatedMass);
		}
	}
}
