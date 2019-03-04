using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class EnterBattleChecker
	{
		public enum MachineValidForUploadResult
		{
			ContainsBadge,
			InvalidForUpload,
			ValidForUpload
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineBuilder machineBuilder
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
		internal BuildHistoryManager buildHistoryManager
		{
			private get;
			set;
		}

		[Inject]
		internal CPUExceededDisplay cpuExceededTracker
		{
			private get;
			set;
		}

		[Inject]
		internal GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		[Inject]
		internal GenericInfoDisplay infoDisplay
		{
			private get;
			set;
		}

		[Inject]
		internal LeagueBadgesEditModeCount leagueBadgeCount
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorGraphUpdater graphUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal WeaponTypeEditModeCount weaponTypeCount
		{
			private get;
			set;
		}

		private void ReturnDetachedCubes()
		{
			IList<InstantiatedCube> disconnectedCubeListCopy = (IList<InstantiatedCube>)graphUpdater.GetDisconnectedCubeListCopy();
			RemoveCubes(disconnectedCubeListCopy);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SaveThenOtherStuff);
		}

		private IEnumerator SaveThenOtherStuff()
		{
			yield return autoSaveController.PerformSave();
			buildHistoryManager.ActuallyClearBuildHistory();
		}

		private void RemoveOverCPUCubes()
		{
			IList<InstantiatedCube> overCPUCubes = cpuExceededTracker.GetOverCPUCubes();
			RemoveCubes(overCPUCubes);
			cpuExceededTracker.ClearOverCPUCubes();
		}

		private void RemoveCubes(IList<InstantiatedCube> cubes)
		{
			for (int i = 0; i < cubes.Count; i++)
			{
				machineBuilder.RemoveCube(cubes[i]);
			}
		}

		private void DisplayNoCubesDialog()
		{
			GenericErrorData data = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strNoCubesInBay"), StringTableBase<StringTable>.Instance.GetString("strNoCubesInBayBattle"));
			infoDisplay.ShowInfoDialogue(data);
		}

		private void DisplayDetachedCubesDialog()
		{
			GenericErrorData data = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strDisconnectedCubes"), StringTableBase<StringTable>.Instance.GetString("strDisconnectedCubesError"), StringTableBase<StringTable>.Instance.GetString("strOK"), StringTableBase<StringTable>.Instance.GetString("strCancel"), ReturnDetachedCubes, null);
			infoDisplay.ShowInfoDialogue(data);
		}

		private void DisplayWeaponsOverLimitDialog()
		{
			GenericErrorData data = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strMaxWeaponCategories"), StringTableBase<StringTable>.Instance.GetString("strMaxWeaponCategoriesError"));
			infoDisplay.ShowInfoDialogue(data);
		}

		private void DisplayExcessCPUDialog()
		{
			GenericErrorData data = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCPULoadExceeded"), StringTableBase<StringTable>.Instance.GetString("strCPUExceededBattle"), StringTableBase<StringTable>.Instance.GetString("strOK"), StringTableBase<StringTable>.Instance.GetString("strCancel"), RemoveOverCPUCubes, null);
			infoDisplay.ShowInfoDialogue(data);
		}

		public bool IsMachineValidForBattle()
		{
			if (machineMap.GetNumberCubes() == 0)
			{
				DisplayNoCubesDialog();
				return false;
			}
			if (graphUpdater.AreCubesDisconnected())
			{
				DisplayDetachedCubesDialog();
				return false;
			}
			if (cpuExceededTracker.AreCubesOverCPU())
			{
				DisplayExcessCPUDialog();
				return false;
			}
			if (weaponTypeCount.IsOverWeaponLimit())
			{
				DisplayWeaponsOverLimitDialog();
				return false;
			}
			return true;
		}

		public MachineValidForUploadResult IsMachineValidForUpload()
		{
			if (machineMap.GetNumberCubes() == 0)
			{
				return MachineValidForUploadResult.InvalidForUpload;
			}
			if (leagueBadgeCount.DoesContainLeagueBadge())
			{
				return MachineValidForUploadResult.ContainsBadge;
			}
			if (graphUpdater.AreCubesDisconnected())
			{
				return MachineValidForUploadResult.InvalidForUpload;
			}
			if (cpuExceededTracker.AreCubesOverCPU())
			{
				return MachineValidForUploadResult.InvalidForUpload;
			}
			return MachineValidForUploadResult.ValidForUpload;
		}
	}
}
