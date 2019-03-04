using Simulation.Hardware.Weapons;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Simulation
{
	internal sealed class GridAllignedLineCheck
	{
		internal class GridAlignedCheckDependency
		{
			public Vector3 hitPoint
			{
				get;
				set;
			}

			public Vector3 startPosition
			{
				get;
				private set;
			}

			public Vector3 direction
			{
				get;
				private set;
			}

			public float range
			{
				get;
				private set;
			}

			public IMachineMap machineMap
			{
				get;
				private set;
			}

			public Rigidbody hitRigidBody
			{
				get;
				private set;
			}

			public TargetType targetType
			{
				get;
				private set;
			}

			public Byte3? cubeToIgnore
			{
				get;
				private set;
			}

			public void Populate(Vector3 _hitPoint, Rigidbody _hitRigidBody, Vector3 _startPosition, Vector3 _direction, float _range, IMachineMap _machineMap, TargetType _targetType, Byte3? _cubeToIgnore)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				hitPoint = _hitPoint;
				startPosition = _startPosition;
				direction = _direction;
				range = _range;
				machineMap = _machineMap;
				hitRigidBody = _hitRigidBody;
				targetType = _targetType;
				cubeToIgnore = _cubeToIgnore;
			}
		}

		internal struct GridAlignedHitResult
		{
			public Byte3 hitGridPos;
		}

		private const float MaxGridSteps = 255f;

		private static int s_foundCubesCount;

		private static HitResult[] s_hitResults;

		private static GridAlignedCheckDependency s_dependency;

		private static Predicate<Int3> s_predicate;

		[CompilerGenerated]
		private static Predicate<Int3> _003C_003Ef__mg_0024cache0;

		public static int GetCubeInGridStepLine(GridAlignedCheckDependency dependency, HitResult[] hitResults)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = Quaternion.Inverse(dependency.hitRigidBody.get_rotation());
			Vector3 pos = val * (dependency.hitPoint - dependency.hitRigidBody.get_position());
			Vector3 val2 = GridScaleUtility.InverseWorldScale(pos, dependency.targetType);
			Vector3 val3 = val * dependency.direction;
			float num = GridScaleUtility.InverseWorldScale(dependency.range - Vector3.Distance(dependency.startPosition, dependency.hitPoint), dependency.targetType);
			num = Mathf.Min(num, 255f);
			Ray ray = default(Ray);
			ray._002Ector(val2, val3.get_normalized());
			ray.set_origin(ray.get_origin() - 0.1f * ray.get_direction());
			s_foundCubesCount = 0;
			s_hitResults = hitResults;
			s_dependency = dependency;
			if (s_predicate == null)
			{
				s_predicate = TestFunc;
			}
			VoxelRaycast.Cast(ray, s_predicate, num);
			s_hitResults = null;
			s_dependency = null;
			return s_foundCubesCount;
		}

		private static bool TestFunc(Int3 testPos)
		{
			MachineCell cellAt = s_dependency.machineMap.GetCellAt(testPos);
			if (cellAt == null || cellAt.info.isDestroyed)
			{
				return false;
			}
			if (s_dependency.cubeToIgnore.HasValue)
			{
				Byte3? cubeToIgnore = s_dependency.cubeToIgnore;
				if (cubeToIgnore.HasValue && cubeToIgnore.GetValueOrDefault() == cellAt.info.gridPos)
				{
					return false;
				}
			}
			if (s_foundCubesCount == 0 || s_hitResults[s_foundCubesCount - 1].gridHit.hitGridPos != (Byte3)testPos)
			{
				s_hitResults[s_foundCubesCount].gridHit.hitGridPos = (Byte3)testPos;
				s_foundCubesCount++;
			}
			return s_foundCubesCount == s_hitResults.Length;
		}
	}
}
