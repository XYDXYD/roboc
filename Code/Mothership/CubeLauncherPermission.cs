using Fabric;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;

namespace Mothership
{
	internal class CubeLauncherPermission : IInitialize, ICubeLauncherPermission
	{
		private float _localCPULoad = 4.2949673E+09f;

		protected InvalidPlacementObservable _invalidPlacementObservable;

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeTypeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal WeaponTypeEditModeCount weaponTypeCounts
		{
			private get;
			set;
		}

		[Inject]
		internal MirrorMode mirrorMode
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
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		public event Action<CubeTypeID> AttemptPlaceCubeNoneLeft = delegate
		{
		};

		public event Action AttemptPlaceCubeOverLimit = delegate
		{
		};

		public event Action AttemptPlaceWeaponsOverLimit = delegate
		{
		};

		public CubeLauncherPermission(InvalidPlacementObservable invalidPlacementObservable)
		{
			_invalidPlacementObservable = invalidPlacementObservable;
		}

		void IInitialize.OnDependenciesInjected()
		{
			cpuPower.RegisterOnCPULoadChanged(OnCPULoadChange);
		}

		public virtual bool CheckAndReportCanPlaceCube(GhostCube ghostCube)
		{
			CubeTypeID currentCube = ghostCube.currentCube;
			ICubeCaster cubeCaster = ghostCube.cubeCaster;
			if (cubeCaster.requiresMirrorMode && !mirrorMode.IsEnabled)
			{
				return false;
			}
			if (cubeCaster.canPlace && !cubeCaster.outSideTheGrid && cubeCaster.isAdjacentSuitableCube)
			{
				return CheckNonPlacementRestrictions(currentCube);
			}
			if (cubeCaster.ghostIntersectsCubes)
			{
				InvalidPlacementType invalidPlacementType = InvalidPlacementType.AttemptPlaceCube_OverlappingCubes;
				_invalidPlacementObservable.Dispatch(ref invalidPlacementType);
			}
			else if (cubeCaster.ghostIntersectsGhost)
			{
				InvalidPlacementType invalidPlacementType2 = InvalidPlacementType.AttemptPlaceCube_OverlappingMirrorGhost;
				_invalidPlacementObservable.Dispatch(ref invalidPlacementType2);
			}
			return false;
		}

		public virtual bool FinalPlacementCheck(GhostCube ghostCube)
		{
			CubeTypeData cubeTypeData = ghostCube.cubeList.CubeTypeDataOf(ghostCube.currentCube);
			if (cubeTypeData != null && cubeTypeData.cubeData.buildModeVisibility == BuildVisibility.Tutorial)
			{
				return false;
			}
			return true;
		}

		public bool CheckNonPlacementRestrictions(CubeTypeID selectedCubeID)
		{
			if (cubeInventory.IsCubeOwned(selectedCubeID))
			{
				if (WillBeWithinRanking(selectedCubeID))
				{
					if (!IsPlacingWeaponOverLimit(selectedCubeID))
					{
						return true;
					}
					InvalidPlacementType invalidPlacementType = InvalidPlacementType.AttemptPlaceCube_MaxWeaponCategories;
					_invalidPlacementObservable.Dispatch(ref invalidPlacementType);
					this.AttemptPlaceWeaponsOverLimit();
				}
				else
				{
					InvalidPlacementType invalidPlacementType2 = InvalidPlacementType.AttemptPlaceCube_OverCPULimit;
					_invalidPlacementObservable.Dispatch(ref invalidPlacementType2);
					this.AttemptPlaceCubeOverLimit();
				}
			}
			else
			{
				if (mirrorMode.IsEnabled)
				{
					InvalidPlacementType invalidPlacementType3 = InvalidPlacementType.AttemptPlaceCube_LockedMirror;
					_invalidPlacementObservable.Dispatch(ref invalidPlacementType3);
				}
				else
				{
					InvalidPlacementType invalidPlacementType4 = InvalidPlacementType.AttemptPlaceCube_NoneLeft;
					_invalidPlacementObservable.Dispatch(ref invalidPlacementType4);
					this.AttemptPlaceCubeNoneLeft(selectedCubeID);
				}
				EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_GUI_KubeMenuError", 0);
			}
			return false;
		}

		private void OnCPULoadChange(uint cpuLoad)
		{
			_localCPULoad = (float)(double)cpuLoad;
		}

		private bool WillBeWithinRanking(CubeTypeID selectedCubeID)
		{
			if (_localCPULoad != 4.2949673E+09f)
			{
				CubeTypeData cubeTypeData = cubeTypeInventory.CubeTypeDataOf(selectedCubeID);
				uint cpuRating = cubeTypeData.cubeData.cpuRating;
				if (cubeTypeData.cubeData.itemType == ItemType.Cosmetic)
				{
					uint num = cpuPower.MaxCosmeticCpuPool - cpuPower.CurrentCosmeticCpuPool;
					if (cpuRating == 0 || _localCPULoad + (float)(double)cpuRating + (float)(double)cpuPower.CurrentCosmeticCpuPool <= (float)(double)(cpuPower.MaxMegabotCpuPower + cpuPower.MaxCosmeticCpuPool))
					{
						return true;
					}
					return false;
				}
				if (cpuRating == 0 || _localCPULoad + (float)(double)cpuRating <= (float)(double)cpuPower.MaxMegabotCpuPower)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private bool IsPlacingWeaponOverLimit(CubeTypeID selectedCubeID)
		{
			ItemDescriptor cubeItemDescriptor = cubeTypeInventory.GetCubeItemDescriptor(selectedCubeID);
			if (cubeItemDescriptor != null && cubeItemDescriptor.isActivable)
			{
				return weaponTypeCounts.WillBeOverWeaponLimit(cubeItemDescriptor);
			}
			return false;
		}
	}
}
