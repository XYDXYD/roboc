using Mothership.Garage.Thumbnail;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;
using Utility;

namespace Mothership
{
	internal sealed class AutoSaveController : IWaitForFrameworkDestruction, IInitialize, IAutoSaveController
	{
		private bool _saveRequired;

		[Inject]
		internal IMachineBuilder machineBuilder
		{
			private get;
			set;
		}

		[Inject]
		internal MachineMover machineMover
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColorUpdater colorUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal PaintFillController paintFillController
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

		[Inject]
		internal WeaponOrderManager weaponOrderManager
		{
			private get;
			set;
		}

		[Inject]
		public IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		public ThumbnailManager thumbnailManager
		{
			private get;
			set;
		}

		[Inject]
		public GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		public void FlagDataDirty()
		{
			Console.Log("AutoSaveController: data has changed! A save will be required");
			_saveRequired = true;
		}

		IEnumerator IAutoSaveController.PerformSave()
		{
			Console.Log("AutoSaveController: performing a save");
			yield return SaveInternal();
		}

		IEnumerator IAutoSaveController.PerformSaveButOnlyIfNecessary()
		{
			if (_saveRequired)
			{
				yield return SaveInternal();
			}
			else
			{
				Console.Log("AutoSaveController: Skipping save as nothing changed");
			}
		}

		private IEnumerator SaveInternal()
		{
			yield return CheckWeaponOrderIsAvailable();
			yield return SaveMachineLayoutWithErrorMessage();
			yield return SaveMachineColorsWithErrorMessage();
			yield return SaveMachineThumbnail();
			yield return garagePresenter.RefreshCurrentRobotDataEnumerator();
			_saveRequired = false;
		}

		private IEnumerator CheckWeaponOrderIsAvailable()
		{
			uint garageSlot = garagePresenter.currentGarageSlot;
			Console.Log("AutoSaveController: Making sure weapon order is loaded..");
			if (!weaponOrderManager.WeaponOrderSet)
			{
				yield return weaponOrderManager.CacheWeaponOrder(garageSlot);
			}
			Console.Log("AutoSaveController: Weapon order set");
		}

		private IEnumerator SaveMachineLayoutWithErrorMessage()
		{
			Console.Log("AutoSaveController: Saving Layout");
			loadingIconPresenter.NotifyLoading("Autosaving");
			MachineModel model = machineMap.BuildMachineLayoutModel();
			ISaveMachineRequest saveMachineRequest = serviceFactory.Create<ISaveMachineRequest, SaveMachineDependency>(new SaveMachineDependency(model, weaponOrderManager.weaponOrder));
			TaskService<SaveMachineResult> requestTask = saveMachineRequest.AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				loadingIconPresenter.NotifyLoadingDone("Autosaving");
				HandleSavingFinishedErrorDisplay((ValidationErrorCode)requestTask.result.errorCode);
				Console.Log("AutoSaveController: Saving Layout complete");
			}
			else
			{
				loadingIconPresenter.NotifyLoadingDone("Autosaving");
				ErrorWindow.ShowServiceErrorWindow(requestTask.behaviour);
				Console.Log("AutoSaveController: Saving Layout failed");
			}
		}

		private IEnumerator SaveMachineColorsWithErrorMessage()
		{
			Console.Log("AutoSaveController: Saving Color Map");
			loadingIconPresenter.NotifyLoading("Autosaving");
			MachineModel model = machineMap.BuildMachineLayoutModel();
			uint garageSlot = garagePresenter.currentGarageSlot;
			ISaveMachineColorRequest saveMachineColorRequest = serviceFactory.Create<ISaveMachineColorRequest, SaveMachineColorDependency>(new SaveMachineColorDependency(garageSlot, model.GetCompressedRobotColorData()));
			TaskService requestTask = saveMachineColorRequest.AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				loadingIconPresenter.NotifyLoadingDone("Autosaving");
				Console.Log("AutoSaveController: Saving Color Map COMPLETE");
			}
			else
			{
				loadingIconPresenter.NotifyLoadingDone("Autosaving");
				ErrorWindow.ShowServiceErrorWindow(requestTask.behaviour);
				Console.Log("AutoSaveController: Saving Color Map failed");
			}
		}

		private IEnumerator SaveMachineThumbnail()
		{
			Console.Log("AutoSaveController: Saving thumbnail");
			uint garageSlot = garagePresenter.currentGarageSlot;
			yield return thumbnailManager.UpdateGarageThumbnail(garageSlot);
			yield return null;
			Console.Log("AutoSaveController: Saving thumbnail complete");
		}

		void IInitialize.OnDependenciesInjected()
		{
			if (machineBuilder != null)
			{
				machineBuilder.OnPlaceCube += delegate
				{
					FlagDataDirty();
				};
				machineBuilder.OnDeleteCube += delegate
				{
					FlagDataDirty();
				};
			}
			if (machineMover != null)
			{
				machineMover.OnMachineMoved += delegate
				{
					FlagDataDirty();
				};
			}
			if (colorUpdater != null)
			{
				colorUpdater.OnCubePainted += FlagDataDirty;
			}
			if (paintFillController != null)
			{
				paintFillController.OnMachinePainted += FlagDataDirty;
			}
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			if (machineBuilder != null)
			{
				machineBuilder.OnPlaceCube -= delegate
				{
					FlagDataDirty();
				};
				machineBuilder.OnDeleteCube -= delegate
				{
					FlagDataDirty();
				};
			}
			if (machineMover != null)
			{
				machineMover.OnMachineMoved -= delegate
				{
					FlagDataDirty();
				};
			}
			if (colorUpdater != null)
			{
				colorUpdater.OnCubePainted -= FlagDataDirty;
			}
			if (paintFillController != null)
			{
				paintFillController.OnMachinePainted -= FlagDataDirty;
			}
		}

		private void HandleSavingFinishedErrorDisplay(ValidationErrorCode errorCode)
		{
			switch (errorCode)
			{
			case ValidationErrorCode.SUCCESS:
				break;
			case ValidationErrorCode.INSUFFICIENT_CUBES:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strSaveError"), StringTableBase<StringTable>.Instance.GetString("strCouldNotSaveRobotDontOwnCubes")));
				break;
			case ValidationErrorCode.INSUFFICIENT_CPU:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCPUExceeded"), StringTableBase<StringTable>.Instance.GetString("strCPUExceedSaveError")));
				break;
			case ValidationErrorCode.TOO_MANY_WEAPON_TYPES:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strSaveError"), StringTableBase<StringTable>.Instance.GetString("strMaxWeaponCategoriesError")));
				break;
			case ValidationErrorCode.INVALID_CUBE_POSITION:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strSaveError"), StringTableBase<StringTable>.Instance.GetString("strInvalidOrient")));
				break;
			case ValidationErrorCode.MULTIPLE_PILOT_SEAT:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strSaveError"), StringTableBase<StringTable>.Instance.GetString("strMultiplePilot")));
				break;
			default:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strSaveError"), StringTableBase<StringTable>.Instance.GetString("strCouldNotSaveRobot")));
				break;
			}
		}
	}
}
