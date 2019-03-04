using Services.Mothership;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;

namespace Mothership
{
	internal sealed class WeaponOrderManager : IInitialize, IWaitForFrameworkDestruction
	{
		private WeaponOrderMothership _weaponOrder;

		private bool _weaponsAlreadyAdded;

		[Inject]
		internal WeaponReorderDisplay weaponReorderDisplay
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineBuilder builder
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorBuilder machineEditorBuilder
		{
			private get;
			set;
		}

		[Inject]
		internal IAutoSaveController autoSaveController
		{
			private get;
			set;
		}

		[Inject]
		internal WeaponTypeEditModeCount weaponTypeEditModeCount
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		public WeaponOrderMothership weaponOrder => _weaponOrder;

		public bool WeaponOrderSet => _weaponOrder != null;

		void IInitialize.OnDependenciesInjected()
		{
			machineEditorBuilder.OnMachineBuilt += InitialiseMachineWeaponOrder;
			weaponReorderDisplay.onWeaponsReordered += ReorderWeapons;
			weaponReorderDisplay.onWeaponsDelete += RemoveFromMachine;
			builder.OnPlaceCube += AddCube;
			builder.OnDeleteCube += DeleteCube;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineEditorBuilder.OnMachineBuilt -= InitialiseMachineWeaponOrder;
			weaponReorderDisplay.onWeaponsReordered -= ReorderWeapons;
			weaponReorderDisplay.onWeaponsDelete -= RemoveFromMachine;
			builder.OnPlaceCube -= AddCube;
			builder.OnDeleteCube -= DeleteCube;
		}

		public IEnumerator CacheWeaponOrder(uint slot)
		{
			loadingIconPresenter.NotifyLoading("LoadWeaponOrder");
			ILoadMachineRequest loadMachineRequest = serviceFactory.Create<ILoadMachineRequest, uint?>(slot);
			TaskService<LoadMachineResult> loadMachineRequestTask = new TaskService<LoadMachineResult>(loadMachineRequest);
			yield return new HandleTaskServiceWithError(loadMachineRequestTask, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadWeaponOrder");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadWeaponOrder");
			}).GetEnumerator();
			_weaponOrder = loadMachineRequestTask.result.weaponOrder;
		}

		private void InitialiseMachineWeaponOrder(uint garageSlot)
		{
			loadingIconPresenter.NotifyLoading("LoadWeaponOrder");
			IServiceRequest serviceRequest = serviceFactory.Create<ILoadMachineRequest, uint?>(garageSlot).SetAnswer(new ServiceAnswer<LoadMachineResult>(delegate(LoadMachineResult result)
			{
				loadingIconPresenter.NotifyLoadingDone("LoadWeaponOrder");
				_weaponOrder = result.weaponOrder;
				CheckWeaponsAlreadyAddedOnMachine();
			}, OnLoadFailed));
			serviceRequest.Execute();
		}

		private void OnLoadFailed(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("LoadWeaponOrder");
			ErrorWindow.ShowServiceErrorWindow(behaviour, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadWeaponOrder");
			});
		}

		private void AddCube(InstantiatedCube cube)
		{
			PersistentCubeData persistentCubeData = cube.persistentCubeData;
			ItemDescriptor itemDescriptor = persistentCubeData.itemDescriptor;
			if (!(itemDescriptor != null) || !itemDescriptor.isActivable)
			{
				return;
			}
			int itemDescriptorKey = itemDescriptor.GenerateKey();
			if (!_weaponOrder.Contains(itemDescriptor))
			{
				if (_weaponsAlreadyAdded)
				{
					weaponReorderDisplay.ShowNewItemDescriptor(itemDescriptorKey);
				}
				else
				{
					_weaponOrder.Set(0, itemDescriptorKey);
				}
			}
			_weaponsAlreadyAdded = true;
		}

		private void DeleteCube(InstantiatedCube cubeToDelete)
		{
			PersistentCubeData persistentCubeData = cubeToDelete.persistentCubeData;
			ItemDescriptor itemDescriptor = persistentCubeData.itemDescriptor;
			if (itemDescriptor != null && itemDescriptor.isActivable && weaponTypeEditModeCount.GetItemDescriptorCount(itemDescriptor) == 0)
			{
				int itemDescriptorKey = itemDescriptor.GenerateKey();
				_weaponOrder.Remove(itemDescriptorKey);
				CheckWeaponsAlreadyAddedOnMachine();
			}
		}

		private void ReorderWeapons(WeaponReorderButton[] buttons)
		{
			_weaponOrder.Clear();
			for (int i = 0; i < buttons.Length; i++)
			{
				WeaponReorderButton weaponReorderButton = buttons[i];
				if (weaponReorderButton.buttonState != WeaponReorderButton.ButtonState.Delete)
				{
					int itemDescriptorKey = weaponReorderButton.itemDescriptorKey;
					_weaponOrder.Set(i, itemDescriptorKey);
				}
			}
			autoSaveController.FlagDataDirty();
		}

		private void RemoveFromMachine(int itemDescriptorKey)
		{
			if (itemDescriptorKey != 0)
			{
				builder.RemoveAllCubesByItemDescriptor(itemDescriptorKey);
			}
		}

		private void CheckWeaponsAlreadyAddedOnMachine()
		{
			if (_weaponOrder.GetFirstItemDescriptorKey() > 0)
			{
				_weaponsAlreadyAdded = true;
			}
			else
			{
				_weaponsAlreadyAdded = false;
			}
		}
	}
}
