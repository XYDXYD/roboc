using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class DiscShieldFactory
	{
		private readonly Func<ShieldEntity> _onShieldAllocation;

		private float _shieldHeight = -1f;

		private string _currentShieldPrefabName;

		private BuildShieldParametersData _parametersData;

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		public ShieldEntityObjectPool shieldObjectPool
		{
			get;
			set;
		}

		[Inject]
		public IEntityFactory engineRoot
		{
			get;
			set;
		}

		public DiscShieldFactory()
		{
			_onShieldAllocation = GenerateNewShield;
		}

		public ShieldEntity Build(string prefabName, BuildShieldParametersData parametersData, bool hitSomething)
		{
			_currentShieldPrefabName = prefabName;
			_parametersData = parametersData;
			ShieldEntity shieldEntity = shieldObjectPool.Use(_currentShieldPrefabName, _onShieldAllocation);
			if (hitSomething)
			{
				SetDiscShieldValuesIfHitSomething(shieldEntity);
			}
			else
			{
				SetDiscShieldValuesIfHitNothing(shieldEntity);
			}
			return shieldEntity;
		}

		public void PreallocateShield(string prefabName, int numShields)
		{
			_currentShieldPrefabName = prefabName;
			shieldObjectPool.Preallocate(_currentShieldPrefabName, numShields, _onShieldAllocation);
		}

		private void SetDiscShieldValuesIfHitNothing(ShieldEntity shieldEntity)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			GameObject gameObject = shieldEntity.get_gameObject();
			gameObject.get_transform().set_position(_parametersData.position);
			gameObject.get_transform().set_rotation(_parametersData.rotation);
			shieldEntity.SetOwnership(_parametersData.owner, _parametersData.isMine, _parametersData.isOnMyTeam);
			shieldEntity.SetJustSpawned(justSpawned: true);
		}

		private void SetDiscShieldValuesIfHitSomething(ShieldEntity shieldEntity)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			GameObject gameObject = shieldEntity.get_gameObject();
			gameObject.SetActive(false);
			Vector3 hitPoint = _parametersData.hitPoint;
			Vector3 hitNormal = _parametersData.hitNormal;
			Rigidbody rigidBody = _parametersData.rigidBody;
			gameObject.get_transform().set_position(hitPoint + hitNormal * (_shieldHeight / 2f + shieldEntity.GetDiscShieldGroundOffset()));
			Vector3 val = hitPoint - rigidBody.get_worldCenterOfMass();
			Vector3 normalized = val.get_normalized();
			float num = 0f - Vector3.Dot(hitNormal, normalized);
			Vector3 val2 = normalized + hitNormal * num;
			gameObject.get_transform().set_rotation(Quaternion.FromToRotation(gameObject.get_transform().get_up(), val2));
			shieldEntity.SetOwnership(_parametersData.owner, _parametersData.isMine, _parametersData.isOnMyTeam);
			shieldEntity.SetJustSpawned(justSpawned: true);
		}

		private ShieldEntity GenerateNewShield()
		{
			GameObject val = gameObjectFactory.Build(_currentShieldPrefabName);
			val.SetActive(true);
			engineRoot.BuildEntity<DiscShieldEntityDescriptor>(val.GetInstanceID(), (object[])val.GetComponents<MonoBehaviour>());
			ShieldEntity component = val.GetComponent<ShieldEntity>();
			if (_shieldHeight < 0f)
			{
				CalculateShieldHeight(val);
			}
			val.SetActive(false);
			return component;
		}

		private void CalculateShieldHeight(GameObject go)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Collider[] componentsInChildren = go.get_transform().GetComponentsInChildren<Collider>();
			float num = float.MinValue;
			float num2 = float.MaxValue;
			foreach (Collider val in componentsInChildren)
			{
				Bounds bounds = val.get_bounds();
				Vector3 max = bounds.get_max();
				float x = max.x;
				Bounds bounds2 = val.get_bounds();
				Vector3 min = bounds2.get_min();
				float x2 = min.x;
				if (x > num)
				{
					num = x;
				}
				if (x2 < num2)
				{
					num2 = x2;
				}
			}
			_shieldHeight = num - num2;
		}
	}
}
