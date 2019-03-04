using Simulation.Hardware;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class MachineGraphBuilder
	{
		private ICubeFactory _cubeFactory;

		private ICubeList _cubeList;

		private MachineCollidersCluster _machineCluster;

		private InstantiatedCube _cellRoot;

		private IServiceRequestFactory _serviceFactory;

		private SkinnedMeshCreator _skinnedMeshCreator;

		private Dictionary<CubeTypeID, GameObject> _cachedCubeTypeGameObject;

		private FasterList<Collider> _nonClusteredColliders = new FasterList<Collider>(25);

		private Dictionary<Byte3, CubeData> _cubesData = new Dictionary<Byte3, CubeData>();

		private float _totalHealthBoost = 1f;

		private Vector3 _center = Vector3.get_zero();

		private const float OPERATION_TIME = 0.005f;

		private MachineClusterPool _pool;

		private MachineSphereColliderPool _sphereColliderPool;

		public MachineGraphBuilder(ICubeFactory cubeFactory, IServiceRequestFactory serviceFactory, ICubeList cubeList, Dictionary<CubeTypeID, GameObject> cachedCubeTypeGameObject, MachineClusterPool pool, MachineSphereColliderPool sphereColliderPool, SkinnedMeshCreator skinnedCreator)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			_cubeFactory = cubeFactory;
			_serviceFactory = serviceFactory;
			_cubeList = cubeList;
			_cachedCubeTypeGameObject = cachedCubeTypeGameObject;
			_pool = pool;
			_sphereColliderPool = sphereColliderPool;
			_skinnedMeshCreator = skinnedCreator;
		}

		public IEnumerator BuildMachineGraphWithGridLocation(PreloadedMachine p, MachineDTO dto, bool firstBuild, ColorPaletteData _defaultColorPalette, TargetType targetType)
		{
			p.machineMap.SetGridSizeByMachineType(targetType == TargetType.Player);
			if (firstBuild)
			{
				yield return TaskRunnerExtensions.ThreadSafeRunOnSchedule(BuildMachineCubeInstances(dto, p.machineMap), StandardSchedulers.get_multiThreadScheduler());
			}
			yield return BuildMachineCubes(p, _totalHealthBoost, targetType, firstBuild, _defaultColorPalette);
			yield return TaskRunnerExtensions.ThreadSafeRunOnSchedule(ComputeMachineInfo(p, firstBuild, targetType), StandardSchedulers.get_multiThreadScheduler());
			if (firstBuild)
			{
				_machineCluster = new MachineCollidersCluster(_pool);
				yield return TaskRunnerExtensions.ThreadSafeRunOnSchedule(BuildMachineGraph(p, _cellRoot, p.machineMap, _machineCluster), StandardSchedulers.get_multiThreadScheduler());
				yield return TaskRunnerExtensions.ThreadSafeRunOnSchedule(ClusterMachine(p.machineGraph), StandardSchedulers.get_multiThreadScheduler());
			}
		}

		private Bounds SetExtensions(IMachineMap machineMap, MachineGraph machineGraph, TargetType targetType)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return _cellRoot.colliderInfo[0].bounds;
		}

		private IEnumerator ClusterMachine(MachineGraph machineGraph)
		{
			if (_cellRoot.cubeNodeInstance.GetOriginalNeighbours().get_Count() == 0)
			{
				int hashCode = _cellRoot.GetHashCode();
				CubeColliderInfo[] colliderInfo = _cellRoot.colliderInfo;
				_machineCluster.AddLink(hashCode, colliderInfo, hashCode, colliderInfo);
			}
			yield return _machineCluster.Cluster(machineGraph);
			machineGraph.sphere = new MicrobotCollisionSphere(_serviceFactory, _sphereColliderPool, _nonClusteredColliders);
		}

		private IEnumerator BuildMachineCubeInstances(MachineDTO dto, IMachineMap machineMap)
		{
			_cubesData.Clear();
			_center = Vector3.get_zero();
			for (int i = 0; i < dto.cubes.get_Count(); i++)
			{
				CubeData cubeData = dto.cubes.get_Item(i);
				_center += cubeData.gridLocation.ToVector3();
				_cubesData[cubeData.gridLocation] = cubeData;
				uint iID = cubeData.iID;
				if (_cubeList.CubeTypeDataOf(iID) == null)
				{
					continue;
				}
				CubeNodeInstance cubeNodeInstance = new CubeNodeInstance();
				InstantiatedCube instantiatedCube = new InstantiatedCube(cubeNodeInstance, _cubeList.CubeTypeDataOf(iID).cubeData, cubeData.gridLocation, cubeData.rotationIndex);
				_totalHealthBoost += instantiatedCube.persistentCubeData.healthBoost;
				instantiatedCube.paletteIndex = cubeData.paletteIndex;
				cubeNodeInstance.instantiatedCube = instantiatedCube;
				cubeNodeInstance.isDestroyed = cubeData.isDestroyed;
				machineMap.AddCubeAt(cubeData.gridLocation, instantiatedCube);
				List<ConnectionPoint> adjacentCubeLocations = cubeNodeInstance.GetAdjacentCubeLocations();
				for (int j = 0; j < adjacentCubeLocations.Count; j++)
				{
					ConnectionPoint connectionPoint = adjacentCubeLocations[j];
					if (connectionPoint != null)
					{
						Vector3 vec = CubeData.IndexToQuat(cubeData.rotationIndex) * connectionPoint.offset;
						Byte3 key = cubeData.gridLocation + new Byte3(vec);
						_cubesData[key] = cubeData;
					}
				}
			}
			_center /= (float)dto.Count;
			yield break;
		}

		private IEnumerator BuildMachineCubes(PreloadedMachine p, float totalHealthBoost, TargetType targetType, bool firstBuild, ColorPaletteData _defaultColorPalette)
		{
			if (targetType != 0)
			{
				_center.y = 0f;
			}
			IMachineMap machineMap = p.machineMap;
			float num = float.MaxValue;
			FasterList<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			InstantiatedCube[] array = allInstantiatedCubes.ToArrayFast();
			for (int num2 = allInstantiatedCubes.get_Count() - 1; num2 >= 0; num2--)
			{
				InstantiatedCube instantiatedCube = array[num2];
				PersistentCubeData persistentCubeData = instantiatedCube.persistentCubeData;
				GameObject val = BuildAndInitialiseGameObject(instantiatedCube, targetType);
				if (persistentCubeData.isBatchable)
				{
					p.batchableCubes.Add(new SettingUpCube(val.get_transform(), instantiatedCube));
					instantiatedCube.colour = Color32.op_Implicit(_defaultColorPalette[instantiatedCube.paletteIndex].diffuse);
				}
				else
				{
					p.allRenderers.AddRange(val.GetComponentsInChildren<Renderer>(true));
					CollectNonClusteredCollider(instantiatedCube, val);
					MachinePartColorUpdater component = val.GetComponent<MachinePartColorUpdater>();
					component.Initialize(_defaultColorPalette[instantiatedCube.paletteIndex]);
				}
				if (!persistentCubeData.isGoReused)
				{
					val.get_transform().set_parent(p.rbData.get_transform());
					p.allCubeTransforms.Add(val.get_transform());
				}
				if (firstBuild)
				{
					instantiatedCube.SetParams(val.GetComponent<CubeInstance>());
					Vector3 val2 = instantiatedCube.gridPos.ToVector3() - _center;
					float sqrMagnitude = val2.get_sqrMagnitude();
					if (sqrMagnitude < num)
					{
						_cellRoot = instantiatedCube;
						num = sqrMagnitude;
					}
					int num3 = Mathf.FloorToInt((float)persistentCubeData.health * totalHealthBoost);
					int num5 = instantiatedCube.health = (instantiatedCube.initialTotalHealth = num3);
				}
				else if (instantiatedCube.isDestroyed)
				{
					val.SetActive(false);
				}
				machineMap.UpdateGameObject(instantiatedCube, val);
			}
			yield break;
		}

		private GameObject BuildAndInitialiseGameObject(InstantiatedCube instantiatedCube, TargetType targetType)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			PersistentCubeData persistentCubeData = instantiatedCube.persistentCubeData;
			uint iD = persistentCubeData.cubeType.ID;
			GameObject value;
			if (!persistentCubeData.isGoReused)
			{
				value = _cubeFactory.BuildCube(iD, Vector3.get_zero(), Quaternion.get_identity(), targetType);
				float meshScale;
				bool skinned = _skinnedMeshCreator.TryApplySkinnedMesh(iD, value, out meshScale);
				Initialise(instantiatedCube.persistentCubeData, value, skinned, meshScale);
				value.get_transform().SetPositionAndRotation(GridScaleUtility.GridToWorld(instantiatedCube.gridPos, targetType), CubeData.IndexToQuat(instantiatedCube.rotationIndex));
			}
			else if (!_cachedCubeTypeGameObject.TryGetValue(iD, out value))
			{
				value = _cubeFactory.BuildCube(iD, Vector3.get_zero(), Quaternion.get_identity(), targetType);
				_cachedCubeTypeGameObject.Add(iD, value);
				SetColliderInfo(persistentCubeData, value);
			}
			return value;
		}

		private void Initialise(PersistentCubeData cubeData, GameObject go, bool skinned, float meshScale)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (cubeData.itemDescriptor != null)
			{
				SetItemDescriptor(go, cubeData.itemDescriptor);
			}
			go.AddComponent<MachinePartColorUpdater>();
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
			LODGroup val = go.AddComponent<LODGroup>();
			val.SetLODs((LOD[])new LOD[1]
			{
				new LOD(0.007f, componentsInChildren)
			});
			val.RecalculateBounds();
			if (skinned)
			{
				float size = val.get_size();
				val.set_size(size * meshScale);
				val.set_localReferencePoint(Vector3.get_zero());
			}
			SetColliderInfo(cubeData, go);
		}

		private void SetColliderInfo(PersistentCubeData data, GameObject cube)
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			bool flag = data.colliderInfo == null;
			Collider[] componentsInChildren = cube.GetComponentsInChildren<Collider>(true);
			if (flag)
			{
				data.colliderInfo = new CubeColliderInfo[componentsInChildren.Length];
			}
			bool isGoReused = data.isGoReused;
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				Collider val = componentsInChildren[num];
				CubeColliderInfo cubeColliderInfo;
				if (flag)
				{
					cubeColliderInfo = new CubeColliderInfo();
					cubeColliderInfo.isSingularity = val.CompareTag("MachineCollidersCluster.IsSingularity");
					cubeColliderInfo.doNotCluster = (val is WheelCollider || val.CompareTag("MachineCollidersCluster.DoNotCluster"));
					cubeColliderInfo.canNotBeHit = val.CompareTag("MachineCollidersCluster.CanNotHit");
					cubeColliderInfo.isTrigger = val.get_isTrigger();
					cubeColliderInfo.bounds = val.get_bounds();
					data.colliderInfo[num] = cubeColliderInfo;
				}
				else
				{
					cubeColliderInfo = data.colliderInfo[num];
				}
				if (cubeColliderInfo.doNotCluster || cubeColliderInfo.isTrigger)
				{
					data.nonClusteredColliders = true;
				}
				if (!cubeColliderInfo.isTrigger && !cubeColliderInfo.doNotCluster && !cubeColliderInfo.canNotBeHit && !isGoReused)
				{
					GameObject gameObject = val.get_gameObject();
					Object.DestroyImmediate(val);
					if (gameObject.GetComponentsInChildren<Component>(true).Length == 1)
					{
						Object.DestroyImmediate(gameObject);
					}
				}
				else if (isGoReused)
				{
					val.get_gameObject().set_layer(GameLayers.IGNORE_RAYCAST);
				}
			}
		}

		private void CollectNonClusteredCollider(InstantiatedCube instantiatedCube, GameObject cube)
		{
			if (!instantiatedCube.persistentCubeData.nonClusteredColliders)
			{
				return;
			}
			Collider[] componentsInChildren = cube.GetComponentsInChildren<Collider>(true);
			foreach (Collider val in componentsInChildren)
			{
				if (val.get_isTrigger() || val.CompareTag("MachineCollidersCluster.DoNotCluster"))
				{
					_nonClusteredColliders.Add(val);
				}
			}
		}

		private IEnumerator ComputeMachineInfo(PreloadedMachine p, bool firstBuild, TargetType targetType)
		{
			MachineInfo machineInfo = p.machineInfo;
			machineInfo.totalMass = 0f;
			machineInfo.initialCOM = Vector3.get_zero();
			machineInfo.totalCpu = 0u;
			FasterList<InstantiatedCube> allInstantiatedCubes = p.machineMap.GetAllInstantiatedCubes();
			Bounds val = default(Bounds);
			val._002Ector(GridScaleUtility.GridToWorld(allInstantiatedCubes.get_Item(0).gridPos, targetType), Vector3.get_one() * 0.1f);
			for (int i = 0; i < allInstantiatedCubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = allInstantiatedCubes.get_Item(i);
				SetColliderInfo(instantiatedCube, targetType);
				if (firstBuild)
				{
					SetCubeExtentsInMachineMap(instantiatedCube, p.machineMap, targetType);
				}
				CubeColliderInfo[] colliderInfo = instantiatedCube.colliderInfo;
				for (int j = 0; j < colliderInfo.Length; j++)
				{
					val.Encapsulate(colliderInfo[j].bounds);
				}
				if (!instantiatedCube.isDestroyed)
				{
					float num = instantiatedCube.mass * instantiatedCube.physcisMassScalar;
					machineInfo.totalMass += num;
					MachineInfo machineInfo2 = machineInfo;
					machineInfo2.initialCOM += num * GetCubePosition(instantiatedCube, targetType);
				}
				machineInfo.totalCpu += instantiatedCube.persistentCubeData.cpuRating;
				machineInfo.totalHealth += instantiatedCube.initialTotalHealth;
			}
			machineInfo.MachineCenter = val.get_center();
			machineInfo.MachineSize = val.get_size();
			MachineInfo machineInfo3 = machineInfo;
			machineInfo3.initialCOM /= machineInfo.totalMass;
			yield break;
		}

		private Vector3 GetCubePosition(InstantiatedCube cube, TargetType targetType)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			return GridScaleUtility.GridToWorld(cube.gridPos, targetType) + GridScaleUtility.WorldScale(CubeData.IndexToQuat(cube.rotationIndex) * cube.comOffset, targetType);
		}

		private static float GetDamagedCpu(InstantiatedCube cube)
		{
			float num = cube.totalHealth;
			float num2 = cube.health;
			float num3 = num2 / num;
			return num3 * (float)(double)cube.persistentCubeData.cpuRating;
		}

		private void SetColliderInfo(InstantiatedCube instantiatedCube, TargetType targetType)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			CubeColliderInfo[] colliderInfo = instantiatedCube.persistentCubeData.colliderInfo;
			instantiatedCube.colliderInfo = new CubeColliderInfo[colliderInfo.Length];
			Vector3 val = Vector3.get_zero();
			Vector3 position = GridScaleUtility.GridToWorld(instantiatedCube.gridPos, targetType);
			Quaternion rotation = CubeData.IndexToQuat(instantiatedCube.rotationIndex);
			for (int num = colliderInfo.Length - 1; num >= 0; num--)
			{
				CubeColliderInfo cubeColliderInfo = colliderInfo[num];
				CubeColliderInfo cubeColliderInfo2 = new CubeColliderInfo();
				cubeColliderInfo2.isSingularity = cubeColliderInfo.isSingularity;
				cubeColliderInfo2.doNotCluster = cubeColliderInfo.doNotCluster;
				cubeColliderInfo2.canNotBeHit = cubeColliderInfo.canNotBeHit;
				cubeColliderInfo2.isTrigger = cubeColliderInfo.isTrigger;
				cubeColliderInfo2.bounds = UpdateBounds(ref cubeColliderInfo.bounds, ref position, ref rotation);
				cubeColliderInfo2.colliderData = new ColliderData_Box(ref cubeColliderInfo2.bounds, ref position);
				instantiatedCube.colliderInfo[num] = cubeColliderInfo2;
				if (!instantiatedCube.customCOM)
				{
					Vector3 val2 = val;
					Vector3 offset = cubeColliderInfo2.colliderData.offset;
					Vector3 size = cubeColliderInfo2.colliderData.size;
					val = val2 + offset * size.get_magnitude();
				}
			}
			if (!instantiatedCube.customCOM)
			{
				if (colliderInfo.Length > 0)
				{
					val /= (float)colliderInfo.Length;
				}
				instantiatedCube.comOffset = val;
			}
		}

		private Bounds UpdateBounds(ref Bounds bounds, ref Vector3 position, ref Quaternion rotation)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = bounds.get_size();
			val = rotation * val;
			for (int i = 0; i < 3; i++)
			{
				val.set_Item(i, Mathf.Abs(val.get_Item(i)));
			}
			return new Bounds(rotation * bounds.get_center() + position, val);
		}

		private void SetItemDescriptor(GameObject cube, ItemDescriptor itemDescriptor)
		{
			if (itemDescriptor is ModuleDescriptor)
			{
				BaseModuleImplementor component = cube.GetComponent<BaseModuleImplementor>();
				component.SetDescriptor(itemDescriptor);
			}
			else if (itemDescriptor is WeaponDescriptor)
			{
				BaseWeaponImplementor component2 = cube.GetComponent<BaseWeaponImplementor>();
				component2.SetDescriptor(itemDescriptor);
			}
			else if (itemDescriptor is MovementDescriptor)
			{
				BaseMovementImplementor component3 = cube.GetComponent<BaseMovementImplementor>();
				component3.SetDescriptor(itemDescriptor);
			}
		}

		private IEnumerator BuildMachineGraph(PreloadedMachine p, InstantiatedCube cellRoot, IMachineMap machineMap, MachineCollidersCluster machineCluster)
		{
			Queue<CubeNodeInstance> queue = new Queue<CubeNodeInstance>();
			Dictionary<int, CubeNodeInstance> dictionary = new Dictionary<int, CubeNodeInstance>();
			MachineGraph machineGraph = new MachineGraph();
			machineGraph.root = _cellRoot.cubeNodeInstance;
			p.machineGraph = machineGraph;
			int hashCode = cellRoot.gridPos.GetHashCode();
			cellRoot.isConnected = true;
			CubeNodeInstance cubeNodeInstance = cellRoot.cubeNodeInstance;
			dictionary.Add(hashCode, cubeNodeInstance);
			queue.Enqueue(cubeNodeInstance);
			while (queue.Count > 0)
			{
				CubeNodeInstance cubeNodeInstance2 = queue.Dequeue();
				if (cubeNodeInstance2 == null || cubeNodeInstance2.instantiatedCube == null)
				{
					RemoteLogger.Error("currentCube is null inside MachineGraphBuilder", string.Empty, string.Empty);
				}
				int hashCode2 = cubeNodeInstance2.instantiatedCube.gridPos.GetHashCode();
				List<ConnectionPoint> adjacentCubeLocations = cubeNodeInstance2.GetAdjacentCubeLocations();
				for (int i = 0; i < adjacentCubeLocations.Count; i++)
				{
					ConnectionPoint connectionPoint = adjacentCubeLocations[i];
					if (TryGetAdjacentCubeData(cubeNodeInstance2, connectionPoint, out CubeData adjacentCubeDataReturn))
					{
						int hashCode3 = adjacentCubeDataReturn.gridLocation.GetHashCode();
						CubeNodeInstance cubeNodeInstance3;
						if (!dictionary.ContainsKey(hashCode3))
						{
							cubeNodeInstance3 = machineMap.GetCellAt(adjacentCubeDataReturn.gridLocation).info.cubeNodeInstance;
							queue.Enqueue(cubeNodeInstance3);
							cubeNodeInstance3.linkToChair = cubeNodeInstance2;
							dictionary.Add(hashCode3, cubeNodeInstance3);
							cubeNodeInstance3.instantiatedCube.isConnected = true;
						}
						else
						{
							cubeNodeInstance3 = dictionary[hashCode3];
						}
						MachineGraph.MakeLink(cubeNodeInstance2, cubeNodeInstance3, connectionPoint);
						CubeColliderInfo[] colliderInfo = cubeNodeInstance2.instantiatedCube.colliderInfo;
						CubeColliderInfo[] colliderInfo2 = cubeNodeInstance3.instantiatedCube.colliderInfo;
						machineCluster.AddLink(hashCode2, colliderInfo, hashCode3, colliderInfo2);
					}
				}
			}
			yield break;
		}

		private void SetCubeExtentsInMachineMap(InstantiatedCube info, IMachineMap machineMap, TargetType targetType)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			CubeColliderInfo[] colliderInfo = info.colliderInfo;
			CubeColliderInfo[] array = colliderInfo;
			foreach (CubeColliderInfo cubeColliderInfo in array)
			{
				if (cubeColliderInfo.canNotBeHit)
				{
					continue;
				}
				GridScaleUtility.MinMaxBoundsToGrid(cubeColliderInfo.bounds, targetType, out Int3 min, out Int3 max);
				for (int j = min.x; j <= max.x; j++)
				{
					for (int k = min.y; k <= max.y; k++)
					{
						for (int l = min.z; l <= max.z; l++)
						{
							Byte3 @byte = new Byte3((byte)j, (byte)k, (byte)l);
							if (machineMap.IsPosValid(j, k, l) && @byte != info.gridPos)
							{
								machineMap.AddCubeExtentAt(@byte, info, null);
							}
						}
					}
				}
			}
		}

		private bool TryGetAdjacentCubeData(CubeNodeInstance cubeNode, ConnectionPoint connection, out CubeData adjacentCubeDataReturn)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			adjacentCubeDataReturn = null;
			InstantiatedCube instantiatedCube = cubeNode.instantiatedCube;
			if (instantiatedCube.IsDirectionSelectable(Quaternion.get_identity(), connection.direction, connection.offset))
			{
				Quaternion val = CubeData.IndexToQuat(instantiatedCube.rotationIndex);
				Vector3 direction = connection.direction;
				Vector3 val2 = val * direction;
				Vector3 val3 = val * connection.offset;
				Vector3 val4 = instantiatedCube.gridPos.ToVector3() + val2 + val3;
				Byte3 key = new Byte3(val4);
				if (_cubesData.TryGetValue(key, out CubeData value))
				{
					Quaternion val5 = CubeData.IndexToQuat(value.rotationIndex);
					Vector3 localOffset = Quaternion.Inverse(val5) * (val4 - value.gridLocation.ToVector3());
					PersistentCubeData cubeData = _cubeList.CubeTypeDataOf(value.iID).cubeData;
					if (cubeData.IsDirectionSelectable(val5, -val2, localOffset))
					{
						adjacentCubeDataReturn = value;
						return true;
					}
				}
			}
			return false;
		}

		private void CheckMachineBuiltCorrectly(CubeNodeInstance root, IMachineMap map)
		{
			HashSet<CubeNodeInstance> hashSet = new HashSet<CubeNodeInstance>();
			Queue<CubeNodeInstance> queue = new Queue<CubeNodeInstance>();
			FasterList<CubeNodeInstance> val = new FasterList<CubeNodeInstance>();
			queue.Enqueue(root);
			while (queue.Count > 0)
			{
				CubeNodeInstance cubeNodeInstance = queue.Dequeue();
				hashSet.Add(cubeNodeInstance);
				val.FastClear();
				val = cubeNodeInstance.GetNeighboursThatLinkToMe();
				for (int i = 0; i < val.get_Count(); i++)
				{
					queue.Enqueue(val.get_Item(i));
				}
			}
			int numberCubes = map.GetNumberCubes();
			Console.Log("Robot built OK");
		}

		private IColliderData SetColliderData(Collider c)
		{
			if (c is SphereCollider)
			{
				return new ColliderData_Sphere(c);
			}
			if (c is CapsuleCollider)
			{
				return new ColliderData_Capsule(c);
			}
			if (c is BoxCollider)
			{
				return new ColliderData_Box(c);
			}
			if (c is WheelCollider)
			{
				return new ColliderData_Wheel(c);
			}
			return new ColliderData_Generic(c);
		}
	}
}
