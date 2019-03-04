using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal static class WeaponSplashDamageUtility
	{
		internal struct SplashParameters
		{
			public float coneAngle;

			public int additionalHits;

			public Vector3 position;

			public Vector3 direction;

			public float radius;
		}

		internal class HitCubesResult
		{
			public Dictionary<int, Target> playerMachines = new Dictionary<int, Target>();

			public Dictionary<int, Target> teamBases = new Dictionary<int, Target>();

			public Dictionary<int, Target> equalizers = new Dictionary<int, Target>();

			public void Reset()
			{
				playerMachines.Clear();
				teamBases.Clear();
				equalizers.Clear();
			}

			public void TryAddTarget(TargetType targetType, int machineId, ref Target target)
			{
				switch (targetType)
				{
				case TargetType.Environment:
				case TargetType.FusionShield:
					break;
				case TargetType.Player:
					if (!playerMachines.TryGetValue(machineId, out Target value2) || value2.sqrDistance > target.sqrDistance)
					{
						playerMachines[machineId] = target;
					}
					break;
				case TargetType.TeamBase:
					if (!teamBases.TryGetValue(machineId, out Target value3) || value3.sqrDistance > target.sqrDistance)
					{
						teamBases[machineId] = target;
					}
					break;
				case TargetType.EqualizerCrystal:
					if (!equalizers.TryGetValue(machineId, out Target value) || value.sqrDistance > target.sqrDistance)
					{
						equalizers[machineId] = target;
					}
					break;
				}
			}
		}

		internal class HitCubesResultList
		{
			public Dictionary<int, FasterList<InstantiatedCube>> playerMachines = new Dictionary<int, FasterList<InstantiatedCube>>();

			public Dictionary<int, FasterList<InstantiatedCube>> teamBases = new Dictionary<int, FasterList<InstantiatedCube>>();

			public Dictionary<int, FasterList<InstantiatedCube>> equalizers = new Dictionary<int, FasterList<InstantiatedCube>>();

			private FasterList<FasterList<InstantiatedCube>> pooledTargetList = new FasterList<FasterList<InstantiatedCube>>(10);

			private int _listIndex;

			public HitCubesResultList()
			{
				for (int i = 0; i < 10; i++)
				{
					pooledTargetList.Add(new FasterList<InstantiatedCube>(10));
				}
			}

			public void Reset()
			{
				_listIndex = 0;
				playerMachines.Clear();
				teamBases.Clear();
				equalizers.Clear();
			}

			public void AddTarget(TargetType targetType, int hitMachineId, InstantiatedCube target)
			{
				switch (targetType)
				{
				case TargetType.TeamBase:
					if (!teamBases.ContainsKey(hitMachineId))
					{
						teamBases[hitMachineId] = GetList();
					}
					teamBases[hitMachineId].Add(target);
					break;
				case TargetType.Player:
					if (!playerMachines.ContainsKey(hitMachineId))
					{
						playerMachines[hitMachineId] = GetList();
					}
					playerMachines[hitMachineId].Add(target);
					break;
				case TargetType.EqualizerCrystal:
					if (!equalizers.ContainsKey(hitMachineId))
					{
						equalizers[hitMachineId] = GetList();
					}
					equalizers[hitMachineId].Add(target);
					break;
				}
			}

			private FasterList<InstantiatedCube> GetList()
			{
				if (_listIndex < pooledTargetList.get_Count())
				{
					FasterList<InstantiatedCube> val = pooledTargetList.get_Item(_listIndex++);
					val.FastClear();
					return val;
				}
				_listIndex++;
				FasterList<InstantiatedCube> val2 = new FasterList<InstantiatedCube>(10);
				pooledTargetList.Add(val2);
				return val2;
			}
		}

		internal struct Target
		{
			public Vector3 fromExplosion;

			public float sqrDistance;
		}

		internal struct Parameters
		{
			public MachineRootContainer machineRootContainer;

			public RigidbodyDataContainer rigidbodyDataContainer;

			public NetworkMachineManager machineManager;

			internal PlayerMachinesContainer playerMachinesContainer;

			internal PlayerTeamsContainer playerTeamsContainer;

			internal int projectileOwnerTeam;
		}

		private static int explosionLayer = (1 << GameLayers.AICOLLIDERS) | (1 << GameLayers.MCOLLIDERS) | (1 << GameLayers.TEAM_BASE) | (1 << GameLayers.EQUALIZER) | (1 << GameLayers.LOCAL_PLAYER_COLLIDERS);

		private static GridAllignedLineCheck.GridAlignedCheckDependency _gridAlignedCheckDependency = new GridAllignedLineCheck.GridAlignedCheckDependency();

		private static HitResult[] _hitResults = new HitResult[1];

		private static HitCubesResult _hitResult = new HitCubesResult();

		private static HitCubesResultList _hitResultList = new HitCubesResultList();

		public static HitCubesResultList SplashDamageCubesList(ref SplashParameters splashParameters, ref Parameters parameters, ref HitResult hitResultParam)
		{
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			_hitResultList.Reset();
			_hitResult.Reset();
			if (LayerToTargetType.IsTargetDestructible(hitResultParam.targetType) && !hitResultParam.hitOwnBase && !hitResultParam.hitAlly && !hitResultParam.hitSelf)
			{
				IMachineMap machineMap = parameters.machineManager.GetMachineMap(hitResultParam.targetType, hitResultParam.hitTargetMachineId);
				Byte3 hitGridPos = hitResultParam.gridHit.hitGridPos;
				if (splashParameters.radius == 0f)
				{
					ProcessHitResultList(hitResultParam.targetType, hitResultParam.hitTargetMachineId, machineMap, hitGridPos);
					return _hitResultList;
				}
				Rigidbody rigidBodyData = parameters.rigidbodyDataContainer.GetRigidBodyData(hitResultParam.targetType, hitResultParam.hitTargetMachineId);
				Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitGridPos, rigidBodyData, hitResultParam.targetType);
				Vector3 startPosition = cubeWorldPosition - splashParameters.direction * splashParameters.radius * 0.5f;
				ComputeAdditionalConeHit(ref splashParameters, ref parameters, ref hitResultParam, ref startPosition);
			}
			ComputeHitsPerTarget(ref splashParameters, ref parameters, ref hitResultParam);
			CheckSplashDamagePartFromHitCubes(ref splashParameters, ref parameters, _hitResult.playerMachines, TargetType.Player);
			CheckSplashDamagePartFromHitCubes(ref splashParameters, ref parameters, _hitResult.equalizers, TargetType.EqualizerCrystal);
			return _hitResultList;
		}

		private static void ComputeHitsPerTarget(ref SplashParameters splashParameters, ref Parameters parameters, ref HitResult hitResultParam)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = splashParameters.position;
			Collider[] hitColliders;
			int numHits = RaycastUtility.OverlapSphere(ref position, splashParameters.radius, explosionLayer, out hitColliders);
			ComputeClosestCubePerTarget(numHits, hitColliders, ref parameters, ref hitResultParam, ref splashParameters);
		}

		private static void CheckSplashDamagePartFromHitCubes(ref SplashParameters splashParameters, ref Parameters parameters, Dictionary<int, Target> targets, TargetType targetType)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			if (targets.Count > 0)
			{
				using (Dictionary<int, Target>.Enumerator enumerator = targets.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int key = enumerator.Current.Key;
						Target value = enumerator.Current.Value;
						HitResult hitResultParam = default(HitResult);
						hitResultParam.hitTargetMachineId = key;
						hitResultParam.targetType = targetType;
						Vector3 startPosition = splashParameters.position;
						Vector3 val = splashParameters.direction = value.fromExplosion.get_normalized();
						float num = splashParameters.radius * splashParameters.radius * 0.25f;
						if (value.sqrDistance < num)
						{
							startPosition = startPosition + value.fromExplosion - val * splashParameters.radius * 0.5f;
						}
						if (CanSplashDamageGoThrough(startPosition, splashParameters.direction, splashParameters.radius))
						{
							ComputeAdditionalConeHit(ref splashParameters, ref parameters, ref hitResultParam, ref startPosition);
						}
					}
				}
			}
		}

		private static void ComputeClosestCubePerTarget(int numHits, Collider[] colliders, ref Parameters parameters, ref HitResult hitResultParam, ref SplashParameters splashParameters)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = splashParameters.position;
			for (int i = 0; i < numHits; i++)
			{
				Collider val = colliders[i];
				Transform transform = val.get_transform();
				TargetType type = LayerToTargetType.GetType(transform.get_gameObject().get_layer());
				if (!LayerToTargetType.IsTargetDestructible(type))
				{
					continue;
				}
				GameObject machineBoard = GameUtility.GetMachineBoard(transform);
				int machineIdFromRoot = parameters.machineRootContainer.GetMachineIdFromRoot(type, machineBoard);
				if (hitResultParam.targetType == type && hitResultParam.hitTargetMachineId == machineIdFromRoot)
				{
					continue;
				}
				int playerFromMachineId = parameters.playerMachinesContainer.GetPlayerFromMachineId(type, machineIdFromRoot);
				int playerTeam = parameters.playerTeamsContainer.GetPlayerTeam(type, playerFromMachineId);
				if (playerTeam != parameters.projectileOwnerTeam)
				{
					Vector3 val2 = transform.get_position() - position;
					if (val is BoxCollider)
					{
						val2 += transform.get_rotation() * (val as BoxCollider).get_center();
					}
					Target target = default(Target);
					target.fromExplosion = val2;
					target.sqrDistance = val2.get_sqrMagnitude();
					Target target2 = target;
					_hitResult.TryAddTarget(type, machineIdFromRoot, ref target2);
					IMachineMap machineMap = parameters.machineManager.GetMachineMap(type, machineIdFromRoot);
					Rigidbody rigidBodyData = parameters.rigidbodyDataContainer.GetRigidBodyData(type, machineIdFromRoot);
					_gridAlignedCheckDependency.Populate(position, rigidBodyData, position, val2.get_normalized(), splashParameters.radius, machineMap, type, null);
					int cubeInGridStepLine = GridAllignedLineCheck.GetCubeInGridStepLine(_gridAlignedCheckDependency, _hitResults);
					if (cubeInGridStepLine > 0)
					{
						Byte3 hitGridPos = _hitResults[0].gridHit.hitGridPos;
						Vector3 cubeWorldPosition = GridScaleUtility.GetCubeWorldPosition(hitGridPos, parameters.rigidbodyDataContainer.GetRigidBodyData(type, machineIdFromRoot), type);
						target = default(Target);
						target.fromExplosion = cubeWorldPosition - position;
						Vector3 val3 = cubeWorldPosition - position;
						target.sqrDistance = val3.get_sqrMagnitude();
						target2 = target;
						_hitResult.TryAddTarget(type, machineIdFromRoot, ref target2);
					}
				}
			}
		}

		private static bool CanSplashDamageGoThrough(Vector3 startPos, Vector3 direction, float distance)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			int num = (1 << GameLayers.PROPS) | (1 << GameLayers.FUSION_SHIELD);
			bool result = true;
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(startPos, direction, ref val, distance, num | explosionLayer) && IsNotExplodable(val.get_collider()))
			{
				if (val.get_collider().get_tag() == WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG)
				{
					int layer = val.get_collider().get_gameObject().get_layer();
					val.get_collider().get_gameObject().set_layer(GameLayers.IGNORE_RAYCAST);
					distance -= Vector3.Distance(startPos, val.get_point());
					startPos = val.get_point();
					if (Physics.Raycast(startPos, direction, ref val, distance, num | explosionLayer) && IsNotExplodable(val.get_collider()))
					{
						result = false;
					}
					val.get_collider().get_gameObject().set_layer(layer);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static bool IsNotExplodable(Collider c)
		{
			return ((1 << c.get_gameObject().get_layer()) & explosionLayer) == 0;
		}

		private static void ComputeAdditionalConeHit(ref SplashParameters splashParameters, ref Parameters parameters, ref HitResult hitResultParam, ref Vector3 startPosition)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			IMachineMap machineMap = parameters.machineManager.GetMachineMap(hitResultParam.targetType, hitResultParam.hitTargetMachineId);
			Rigidbody rigidBodyData = parameters.rigidbodyDataContainer.GetRigidBodyData(hitResultParam.targetType, hitResultParam.hitTargetMachineId);
			Vector3 direction = splashParameters.direction;
			float num = 360f / (float)splashParameters.additionalHits;
			float num2 = splashParameters.coneAngle * 0.3f;
			for (int i = -1; i < splashParameters.additionalHits; i++)
			{
				if (i >= 0)
				{
					float num3 = Random.Range(splashParameters.coneAngle - num2, splashParameters.coneAngle + num2);
					float num4 = Random.Range((float)i * num - num2, (float)i * num + num2);
					Quaternion val = Quaternion.Euler(0f, num3, 0f);
					direction = val * splashParameters.direction;
					Quaternion val2 = Quaternion.AngleAxis(num4, splashParameters.direction);
					direction = val2 * direction;
				}
				_gridAlignedCheckDependency.Populate(startPosition, rigidBodyData, startPosition, direction, splashParameters.radius, machineMap, hitResultParam.targetType, null);
				int cubeInGridStepLine = GridAllignedLineCheck.GetCubeInGridStepLine(_gridAlignedCheckDependency, _hitResults);
				if (cubeInGridStepLine > 0)
				{
					HitResult hitResult = _hitResults[0];
					ProcessHitResultList(hitResultParam.targetType, hitResultParam.hitTargetMachineId, machineMap, hitResult.gridHit.hitGridPos);
				}
			}
		}

		private static void ProcessHitResultList(TargetType targetType, int hitMachineId, IMachineMap hitMachineMap, Byte3 hitGridPos)
		{
			InstantiatedCube info = hitMachineMap.GetCellAt(hitGridPos).info;
			_hitResultList.AddTarget(targetType, hitMachineId, info);
		}
	}
}
