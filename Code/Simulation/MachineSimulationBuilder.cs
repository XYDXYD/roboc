using Simulation.Hardware.Movement;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class MachineSimulationBuilder : IInitialize, IWaitForFrameworkInitialization
	{
		private Dictionary<CubeTypeID, GameObject> _cachedCubeTypeGameObject = new Dictionary<CubeTypeID, GameObject>();

		private ColorPaletteData _defaultColorPalette;

		private ServiceBehaviour _serviceErrorBehaviour;

		private MachineColliderCollectionData _machineColliderCollectionData = new MachineColliderCollectionData();

		[Inject]
		public ICubeFactory simCubeFactory
		{
			private get;
			set;
		}

		[Inject]
		public ICubeList cubeList
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
		public PhysicsStatusFactory physicsStatus
		{
			private get;
			set;
		}

		[Inject]
		public MachineClusterPool pool
		{
			private get;
			set;
		}

		[Inject]
		public MachineSphereColliderPool sphereColliderPool
		{
			private get;
			set;
		}

		[Inject]
		internal SkinnedMeshCreator skinnedMeshCreator
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColliderCollectionObservable machineColliderCollectionObservable
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			ILoadDefaultColorPaletteRequest loadDefaultColorPaletteRequest = serviceFactory.Create<ILoadDefaultColorPaletteRequest>();
			loadDefaultColorPaletteRequest.SetAnswer(new ServiceAnswer<ColorPaletteData>(delegate(ColorPaletteData defaultPalette)
			{
				_defaultColorPalette = defaultPalette;
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

		public IEnumerator SetupSimulationMachine(PreloadedMachine preloadedMachine)
		{
			MachineGraphBuilder builder = new MachineGraphBuilder(simCubeFactory, serviceFactory, cubeList, _cachedCubeTypeGameObject, pool, sphereColliderPool, skinnedMeshCreator);
			preloadedMachine.machineMap = new MachineMap(preloadedMachine.machineModel.DTO.Count);
			preloadedMachine.allRenderers = new FasterList<Renderer>(50);
			preloadedMachine.allCubeTransforms = new FasterList<Transform>(50);
			preloadedMachine.batchableCubes = new FasterList<SettingUpCube>(preloadedMachine.machineModel.DTO.Count);
			yield return builder.BuildMachineGraphWithGridLocation(preloadedMachine, preloadedMachine.machineModel.DTO, firstBuild: true, _defaultColorPalette, TargetType.Player);
			AutomaticButtonAssignment buttonAssignment = new AutomaticButtonAssignment();
			buttonAssignment.PopulateCubeControls(preloadedMachine.allCubeTransforms);
			Rigidbody rb = preloadedMachine.rbData;
			rb.set_mass(preloadedMachine.machineInfo.totalMass);
			rb.set_centerOfMass(preloadedMachine.machineInfo.initialCOM);
			Transform rbTransform = rb.get_transform();
			CreateCenterGameObject(rbTransform, preloadedMachine.machineInfo);
			GameObject meshGO = MachineMesh.BatchCubes(_defaultColorPalette, rbTransform, preloadedMachine.batchableCubes, TargetType.Player);
			preloadedMachine.allRenderers.AddRange(meshGO.GetComponentsInChildren<Renderer>(true));
		}

		internal IEnumerator SetupSimulationEntity(PreloadedMachine preloadedMachine, TargetType type, int layer)
		{
			preloadedMachine.machineMap = new MachineMap(preloadedMachine.machineModel.DTO.Count);
			preloadedMachine.allRenderers = new FasterList<Renderer>(50);
			preloadedMachine.allCubeTransforms = new FasterList<Transform>(50);
			preloadedMachine.batchableCubes = new FasterList<SettingUpCube>(preloadedMachine.machineModel.DTO.Count);
			MachineGraphBuilder builder = new MachineGraphBuilder(simCubeFactory, serviceFactory, cubeList, _cachedCubeTypeGameObject, pool, sphereColliderPool, skinnedMeshCreator);
			yield return builder.BuildMachineGraphWithGridLocation(preloadedMachine, preloadedMachine.machineModel.DTO, firstBuild: true, _defaultColorPalette, type);
			physicsStatus.MachineBuilt(preloadedMachine.rbData, preloadedMachine.machineMap, type, computeInertiaTensor: false);
			if (type == TargetType.TeamBase)
			{
				DeactivateCubes(preloadedMachine.machineMap.GetAllInstantiatedCubes(), preloadedMachine.machineMap, preloadedMachine.machineGraph);
			}
			Transform rbTransform = preloadedMachine.rbData.get_transform();
			preloadedMachine.machineGraph.cluster.Configure(layer, 4f);
			_machineColliderCollectionData.ResetData(preloadedMachine.machineId);
			preloadedMachine.machineGraph.cluster.CreateGameObjectStructure(rbTransform, _machineColliderCollectionData.NewColliders, _machineColliderCollectionData.RemovedColliders);
			machineColliderCollectionObservable.Dispatch(ref _machineColliderCollectionData);
			MachineMesh.BatchCubes(_defaultColorPalette, rbTransform, preloadedMachine.batchableCubes, type);
			ReparentPropsLayers(preloadedMachine.machineBoard.get_transform());
		}

		private void DeactivateCubes(FasterList<InstantiatedCube> cubes, IMachineMap machineMap, MachineGraph machineGraph)
		{
			InstantiatedCube instantiatedCube = machineGraph.root.instantiatedCube;
			for (int num = cubes.get_Count() - 1; num >= 0; num--)
			{
				InstantiatedCube instantiatedCube2 = cubes.get_Item(num);
				if (instantiatedCube2.GetHashCode() != instantiatedCube.GetHashCode())
				{
					instantiatedCube2.health = 0;
					machineMap.RemoveCube(instantiatedCube2);
					instantiatedCube2.cubeNodeInstance.BreakLinks();
					instantiatedCube2.cubeNodeInstance.isDestroyed = true;
				}
				else
				{
					instantiatedCube2.health = int.MaxValue;
					cubes.UnorderedRemoveAt(num);
				}
			}
			machineGraph.cluster.RemoveLeaves(cubes);
			cubes.Add(instantiatedCube);
		}

		private void CreateCenterGameObject(Transform parent, MachineInfo machineInfo)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject("centerGameObject");
			val.AddComponent<MachineCenter>();
			Transform transform = val.get_transform();
			transform.set_parent(parent);
			transform.set_localPosition(machineInfo.MachineCenter);
			machineInfo.centerTransform = transform;
			GameObject val2 = new GameObject("cameraPivotGameObject");
			transform = val2.get_transform();
			transform.set_parent(parent);
			Vector3 val3 = default(Vector3);
			val3._002Ector(0f, 0f, machineInfo.MachineSize.z * 0.1666f);
			transform.set_localPosition(machineInfo.MachineCenter + val3);
			machineInfo.cameraPivotTransform = transform;
		}

		private void ReparentPropsLayers(Transform propsParent)
		{
			ReparentGameObject[] componentsInChildren = propsParent.GetComponentsInChildren<ReparentGameObject>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].get_transform().set_parent(propsParent);
			}
		}
	}
}
