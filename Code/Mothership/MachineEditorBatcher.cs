using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class MachineEditorBatcher : IInitialize, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private const int START_BATCHING_EVERY_X_CUBES = 300;

		private bool _isMachineBatched;

		private GameObject _meshesHolder;

		private ChunkMeshUpdater _meshUpdater;

		private HashSet<InstantiatedCube> _cubesToBatch = new HashSet<InstantiatedCube>();

		private FasterList<InstantiatedCube> _cubesToUpdate = new FasterList<InstantiatedCube>();

		private FasterList<InstantiatedCube> _invalidCubesToUpdate = new FasterList<InstantiatedCube>();

		private ColorPaletteData _colorPalette;

		private ServiceBehaviour _serviceErrorBehaviour;

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
		internal MachineMover machineMover
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

		public event Action onPreBatching = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			_meshesHolder = new GameObject("OptimizedMesh");
			_meshesHolder.get_transform().SetParent(MachineBoard.Instance.board, false);
			_meshUpdater = _meshesHolder.AddComponent<ChunkMeshUpdater>();
			machineBuilder.OnAllCubesRemoved += ClearResources;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RunEndOfFrameUpdateMachine);
			ILoadDefaultColorPaletteRequest loadDefaultColorPaletteRequest = serviceFactory.Create<ILoadDefaultColorPaletteRequest>();
			loadDefaultColorPaletteRequest.SetAnswer(new ServiceAnswer<ColorPaletteData>(delegate(ColorPaletteData defaultPalette)
			{
				_colorPalette = defaultPalette;
				MachineMesh.CombineMeshes.ColourTextureWidth = _colorPalette.Count * 4;
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

		public void OnFrameworkDestroyed()
		{
			machineBuilder.OnAllCubesRemoved -= ClearResources;
		}

		internal void UpdateOnCubeDeleted(InstantiatedCube cube)
		{
			if (cube.persistentCubeData.isBatchable && !_cubesToBatch.Remove(cube))
			{
				UpdateMeshIndices(cube);
			}
		}

		internal void UpdateOnCubesDeleted(IList<InstantiatedCube> cubes)
		{
			FasterList<InstantiatedCube> val = new FasterList<InstantiatedCube>();
			for (int i = 0; i < cubes.Count; i++)
			{
				InstantiatedCube instantiatedCube = cubes[i];
				if (instantiatedCube.persistentCubeData.isBatchable && !_cubesToBatch.Remove(instantiatedCube))
				{
					val.Add(instantiatedCube);
				}
			}
			if (val.get_Count() > 0)
			{
				UpdateMeshIndices(val);
			}
		}

		internal void UpdateOnCubePlaced(InstantiatedCube cube)
		{
			if (cube.persistentCubeData.isBatchable)
			{
				_cubesToBatch.Add(cube);
				if (_cubesToBatch.Count >= 300)
				{
					FasterList<SettingUpCube> val = new FasterList<SettingUpCube>();
					GetInfoToSetupMachine(machineMap, _cubesToBatch, val);
					this.onPreBatching();
					MachineMesh.StartBatchCubes(_colorPalette, _meshesHolder, _meshUpdater, val, TargetType.Player);
					_cubesToBatch.Clear();
				}
			}
		}

		public void ClearResources()
		{
			_isMachineBatched = false;
			MachineMesh.ClearDictionary();
			for (int i = 0; i < _meshesHolder.get_transform().get_childCount(); i++)
			{
				Object.Destroy(_meshesHolder.get_transform().GetChild(i).get_gameObject());
			}
			_cubesToBatch.Clear();
		}

		internal void StartBatching(FasterList<InstantiatedCube> robotInstantiatedCubes, FasterList<GameObject> robotGameObjects)
		{
			ClearResources();
			FasterList<SettingUpCube> val = new FasterList<SettingUpCube>();
			GetInfoToSetupMachine(robotInstantiatedCubes, robotGameObjects, val);
			if (val.get_Count() > 0)
			{
				MachineMesh.StartBatchCubes(_colorPalette, _meshesHolder, _meshUpdater, val, TargetType.Player);
			}
			_isMachineBatched = true;
		}

		public void StartBatching()
		{
			ClearResources();
			FasterList<SettingUpCube> val = new FasterList<SettingUpCube>();
			GetInfoToSetupMachine(machineMap, machineMap.GetAllInstantiatedCubes(), val);
			if (val.get_Count() > 0)
			{
				this.onPreBatching();
				MachineMesh.StartBatchCubes(_colorPalette, _meshesHolder, _meshUpdater, val, TargetType.Player);
			}
			_isMachineBatched = true;
		}

		private void GetInfoToSetupMachine(FasterList<InstantiatedCube> allCubes, FasterList<GameObject> allGameObjects, FasterList<SettingUpCube> allCubesToSetup)
		{
			for (int i = 0; i < allCubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = allCubes.get_Item(i);
				if (instantiatedCube.persistentCubeData.isBatchable)
				{
					GameObject val = allGameObjects.get_Item(i);
					val.GetComponent<CubeColorUpdater>().Reset();
					allCubesToSetup.Add(new SettingUpCube(val.get_transform(), instantiatedCube));
				}
			}
		}

		private void GetInfoToSetupMachine(IMachineMap machineMap, HashSet<InstantiatedCube> allCubes, FasterList<SettingUpCube> allCubesToSetup)
		{
			HashSet<InstantiatedCube>.Enumerator enumerator = allCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				try
				{
					if (current.persistentCubeData.isBatchable)
					{
						GameObject cubeAt = machineMap.GetCubeAt(current.gridPos);
						cubeAt.GetComponent<CubeColorUpdater>().Reset();
						allCubesToSetup.Add(new SettingUpCube(cubeAt.get_transform(), current));
					}
				}
				catch (Exception ex)
				{
					RemoteLogger.Error("Batching Failed, probably cube outside range", current.gridPos + " " + ex.Message, ex.StackTrace);
				}
			}
		}

		public bool IsBatched(InstantiatedCube cube)
		{
			return cube.persistentCubeData.isBatchable && !_cubesToBatch.Contains(cube) && _isMachineBatched;
		}

		public void UpdateInvalidColor(FasterList<InstantiatedCube> invalidCubes)
		{
			_invalidCubesToUpdate.AddRange(invalidCubes);
		}

		public void UpdateInvalidColor(InstantiatedCube invalidCube)
		{
			_invalidCubesToUpdate.Add(invalidCube);
		}

		private void UpdateMeshIndices(InstantiatedCube cube)
		{
			_cubesToUpdate.Add(cube);
			_meshUpdater.UpdateDestroyTextureInEditor(_cubesToUpdate);
			_cubesToUpdate.FastClear();
		}

		private void UpdateMeshIndices(FasterList<InstantiatedCube> cubes)
		{
			_cubesToUpdate.AddRange(cubes);
		}

		private IEnumerator RunEndOfFrameUpdateMachine()
		{
			while (true)
			{
				yield return null;
				if (_invalidCubesToUpdate.get_Count() > 0)
				{
					_meshUpdater.UpdateColorsInEditor(_invalidCubesToUpdate);
					_invalidCubesToUpdate.FastClear();
				}
				if (_cubesToUpdate.get_Count() > 0)
				{
					_meshUpdater.UpdateDestroyTextureInEditor(_cubesToUpdate);
					_cubesToUpdate.FastClear();
				}
			}
		}

		internal void UpdateMainColor(InstantiatedCube instantiatedCube)
		{
			_meshUpdater.UpdateMainColor(instantiatedCube);
		}

		internal void UpdateMainColor(FasterList<InstantiatedCube> cubes)
		{
			_meshUpdater.UpdateMainColor(cubes);
		}
	}
}
