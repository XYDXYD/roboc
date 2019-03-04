using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal sealed class MachineEditorBuilder : IInitialize, IWaitForFrameworkInitialization
	{
		private GameObject _loadingIcon;

		private ShortCutMode _shortCutMode;

		private ColorPaletteData _colorPalette;

		private ServiceBehaviour _serviceErrorBehaviour;

		[Inject]
		internal IMachineMap machineMap
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
		internal ICubeFactory cubeFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership inputController
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
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
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal RobotDimensionChangedObservable robotDimensionChangedObs
		{
			private get;
			set;
		}

		public event Action<uint> OnMachineBuilt = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			ILoadDefaultColorPaletteRequest loadDefaultColorPaletteRequest = serviceFactory.Create<ILoadDefaultColorPaletteRequest>();
			loadDefaultColorPaletteRequest.SetAnswer(new ServiceAnswer<ColorPaletteData>(delegate(ColorPaletteData defaultPalette)
			{
				_colorPalette = defaultPalette;
			}, delegate(ServiceBehaviour behaviour)
			{
				_serviceErrorBehaviour = behaviour;
			}));
			loadDefaultColorPaletteRequest.Execute();
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			if (_serviceErrorBehaviour != null)
			{
				if (_serviceErrorBehaviour.exceptionThrown != null)
				{
					Debug.LogException(_serviceErrorBehaviour.exceptionThrown);
				}
				else
				{
					Debug.LogException(new Exception("ServiceBehaviour returned from ILoadDefaultColorPaletteRequest was not null"));
				}
				ErrorWindow.ShowServiceErrorWindow(_serviceErrorBehaviour);
				_serviceErrorBehaviour = null;
			}
		}

		public void BuildMachineInGarage(MachineModel machineModel, Action onSuccess, uint garageSlot)
		{
			machineMap.SetGridSizeByMachineType(isRobot: true);
			TaskRunner.get_Instance().Run(BuildEditorMachineCubes(machineModel.DTO, machineMap, onSuccess, garageSlot));
		}

		private IEnumerator BuildEditorMachineCubes(MachineDTO dto, IMachineMap map, Action onSuccess, uint garageSlot)
		{
			bool boundValid = false;
			Bounds bounds = default(Bounds);
			int count = 0;
			FasterListEnumerator<CubeData> enumerator = dto.cubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CubeData cell = enumerator.get_Current();
					if (!map.IsPosValid(cell.gridLocation))
					{
						autoSaveController.FlagDataDirty();
					}
					else
					{
						uint cubeId = cell.iID;
						if (cubeTypeInventory.IsCubeValid(cubeId) && cubeInventory.IsCubeOwned(cubeId))
						{
							CubeNodeInstance cubeNodeInstance = new CubeNodeInstance();
							InstantiatedCube instantiatedCube2 = cubeNodeInstance.instantiatedCube = new InstantiatedCube(cubeNodeInstance, cubeTypeInventory.CubeTypeDataOf(cubeId).cubeData, cell.gridLocation, cell.rotationIndex);
							cubeNodeInstance.isDestroyed = cell.isDestroyed;
							GameObject val = cubeFactory.BuildCube(cell.iID, GridScaleUtility.GridToWorld(cell.gridLocation, TargetType.Player), CubeData.IndexToQuat(cell.rotationIndex), TargetType.Player);
							instantiatedCube2.SetParams(val.GetComponent<CubeInstance>());
							int num = instantiatedCube2.health = (instantiatedCube2.initialTotalHealth = cubeTypeInventory.GetCubeHealth(cell.iID));
							instantiatedCube2.paletteIndex = cell.paletteIndex;
							instantiatedCube2.paletteColor = _colorPalette[instantiatedCube2.paletteIndex];
							val.GetComponent<CubeColorUpdater>().SetColor(instantiatedCube2.paletteColor);
							Collider[] componentsInChildren = val.GetComponentsInChildren<Collider>();
							Collider[] array = componentsInChildren;
							foreach (Collider val2 in array)
							{
								if (boundValid)
								{
									bounds.Encapsulate(val2.get_bounds());
								}
								else
								{
									Bounds bounds2 = val2.get_bounds();
									bounds.set_center(bounds2.get_center());
									Bounds bounds3 = val2.get_bounds();
									bounds.set_extents(bounds3.get_extents());
									Bounds bounds4 = val2.get_bounds();
									bounds.set_max(bounds4.get_max());
									Bounds bounds5 = val2.get_bounds();
									bounds.set_min(bounds5.get_min());
									Bounds bounds6 = val2.get_bounds();
									bounds.set_size(bounds6.get_size());
									boundValid = true;
								}
							}
							map.SetCubeAt(cell.gridLocation, instantiatedCube2, val);
						}
						count++;
						if (count >= 30)
						{
							count = 0;
							yield return null;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			RobotDimensionDependency dep = new RobotDimensionDependency(bounds.get_min(), bounds.get_max());
			robotDimensionChangedObs.Dispatch(ref dep);
			this.OnMachineBuilt(garageSlot);
			onSuccess();
		}

		private void ShowLoadingScreen()
		{
			if (_loadingIcon == null)
			{
				_shortCutMode = inputController.GetShortCutMode();
				inputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				_loadingIcon = gameObjectFactory.Build("GenericLoadingDialog");
				_loadingIcon.set_name("MachineBuilderLoadingScreen");
				_loadingIcon.SetActive(true);
				_loadingIcon.GetComponent<GenericLoadingScreen>().text = StringTableBase<StringTable>.Instance.GetString("strBuildingMachine");
			}
		}

		private void HideLoadingScreen()
		{
			if (_loadingIcon != null)
			{
				Object.Destroy(_loadingIcon);
				inputController.SetShortCutMode(_shortCutMode);
			}
		}
	}
}
