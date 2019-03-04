using Svelto.DataStructures;
using UnityEngine;

namespace Simulation
{
	internal sealed class WheelColliderActivationPerMachine
	{
		private float _prevMass;

		private Rigidbody _rigidbody;

		private FasterList<WheelColliderData> _allWheelColliders = new FasterList<WheelColliderData>();

		private FasterList<WheelColliderData> _supportWheelColliders = new FasterList<WheelColliderData>();

		private FasterList<WheelColliderData> _collidersToActivate = new FasterList<WheelColliderData>();

		private const int MAX_WHEEL_COLLIDERS = 20;

		private const float WHEEL_HEIGHT_THRESHOLD = 0.3f;

		private const float WHEEL_DIST_THRESHOLD = 0.05f;

		private bool _initialised;

		private MachineColliderCollectionData _machineColliderCollectionData = new MachineColliderCollectionData();

		private readonly int _machineId;

		private readonly MachineColliderCollectionObservable _machineColliderCollectionObservable;

		public WheelColliderActivationPerMachine(Rigidbody rb, int machineId, MachineColliderCollectionObservable machineColliderCollectionObservable)
		{
			_rigidbody = rb;
			_machineId = machineId;
			_machineColliderCollectionObservable = machineColliderCollectionObservable;
		}

		public void PhysicsTick()
		{
			_machineColliderCollectionData.ResetData(_machineId);
			ActivatePendingColliders();
			if (HasMassChanged() && OrderCollidersByPriority())
			{
				ActivateWheelColliders();
			}
			if (_machineColliderCollectionData.NewColliders.get_Count() > 0 || _machineColliderCollectionData.RemovedColliders.get_Count() > 0)
			{
				_machineColliderCollectionObservable.Dispatch(ref _machineColliderCollectionData);
			}
		}

		public void AddCubes(WheelColliderData[] wheels)
		{
			foreach (WheelColliderData wheelColliderData in wheels)
			{
				if (!wheelColliderData.support)
				{
					_allWheelColliders.Add(wheelColliderData);
				}
				else
				{
					_supportWheelColliders.Add(wheelColliderData);
				}
			}
		}

		public int RemoveCubes(WheelColliderData[] wheels)
		{
			foreach (WheelColliderData wheelColliderData in wheels)
			{
				if (!wheelColliderData.support)
				{
					_allWheelColliders.Remove(wheelColliderData);
				}
				else
				{
					_supportWheelColliders.Remove(wheelColliderData);
				}
			}
			return _allWheelColliders.get_Count();
		}

		private bool HasMassChanged()
		{
			if (_rigidbody == null)
			{
				return false;
			}
			bool flag = _prevMass != _rigidbody.get_mass();
			if (flag)
			{
				_prevMass = _rigidbody.get_mass();
			}
			return flag;
		}

		private bool OrderCollidersByPriority()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			Vector3 val = Vector3.get_zero();
			for (int i = 0; i < _allWheelColliders.get_Count(); i++)
			{
				Transform cubeRoot = _allWheelColliders.get_Item(i).cubeRoot;
				if (cubeRoot.get_gameObject().get_activeSelf())
				{
					val += cubeRoot.get_localPosition();
					num++;
				}
			}
			if (num > 0)
			{
				val /= (float)num;
			}
			bool result = InsertionSort(_allWheelColliders, val);
			if (!_initialised)
			{
				result = true;
				_initialised = true;
			}
			return result;
		}

		private void ActivateWheelColliders()
		{
			int num = Mathf.Min(_allWheelColliders.get_Count(), 20);
			for (int i = num; i < _allWheelColliders.get_Count(); i++)
			{
				WheelColliderData wheelColliderData = _allWheelColliders.get_Item(i);
				if (wheelColliderData.wheelCollider != null)
				{
					RemoveWheelCollider(wheelColliderData);
				}
			}
			for (int j = 0; j < num; j++)
			{
				WheelColliderData wheelColliderData2 = _allWheelColliders.get_Item(j);
				if (wheelColliderData2.wheelCollider == null)
				{
					_collidersToActivate.Add(wheelColliderData2);
				}
			}
			for (int k = 0; k < _supportWheelColliders.get_Count(); k++)
			{
				_collidersToActivate.Add(_supportWheelColliders.get_Item(k));
			}
		}

		private void RemoveWheelCollider(WheelColliderData wheelData)
		{
			if (wheelData.wheelCollider.get_gameObject().get_activeInHierarchy())
			{
				_machineColliderCollectionData.RemovedColliders.Add(wheelData.wheelCollider);
				Object.Destroy(wheelData.wheelCollider);
				wheelData.wheelCollider = null;
			}
		}

		private void ActivatePendingColliders()
		{
			for (int i = 0; i < _collidersToActivate.get_Count(); i++)
			{
				WheelColliderData wheelColliderData = _collidersToActivate.get_Item(i);
				if (wheelColliderData.wheelCollider == null)
				{
					AddWheelCollider(wheelColliderData);
				}
			}
			_collidersToActivate.FastClear();
		}

		private void AddWheelCollider(WheelColliderData wheelData)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			WheelCollider val = wheelData.wheelObj.get_gameObject().AddComponent<WheelCollider>();
			val.set_mass(wheelData.mass);
			val.set_radius(wheelData.radius);
			val.set_forceAppPointDistance(wheelData.radius);
			val.set_wheelDampingRate(wheelData.wheelDampingRate);
			val.set_suspensionDistance(wheelData.suspensionDistance);
			val.set_center(wheelData.centerOffset);
			JointSpring val2 = default(JointSpring);
			val2.spring = wheelData.spring;
			val2.damper = wheelData.damper;
			val2.targetPosition = wheelData.targetPosition;
			JointSpring suspensionSpring = val2;
			val.set_suspensionSpring(suspensionSpring);
			wheelData.wheelCollider = val;
			for (int i = 0; i < wheelData.wheelColliderInfo.Count; i++)
			{
				wheelData.wheelColliderInfo[i].WheelColliderActivated();
			}
			_machineColliderCollectionData.NewColliders.Add(val);
		}

		private bool InsertionSort(FasterList<WheelColliderData> list, Vector3 wheelsCenter)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			int count = list.get_Count();
			for (int i = 1; i < count; i++)
			{
				int num = i;
				while (num > 0 && Compare(list.get_Item(num - 1), list.get_Item(num), wheelsCenter) > 0)
				{
					WheelColliderData wheelColliderData = list.get_Item(num - 1);
					list.set_Item(num - 1, list.get_Item(num));
					list.set_Item(num, wheelColliderData);
					num--;
					result = true;
				}
			}
			return result;
		}

		private int Compare(WheelColliderData a, WheelColliderData b, Vector3 wheelsCenter)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			bool activeSelf = a.cubeRoot.get_gameObject().get_activeSelf();
			bool activeSelf2 = b.cubeRoot.get_gameObject().get_activeSelf();
			if (activeSelf && !activeSelf2)
			{
				return -1;
			}
			if (activeSelf2 && !activeSelf)
			{
				return 1;
			}
			if (a.priority < b.priority)
			{
				return -1;
			}
			if (b.priority < a.priority)
			{
				return 1;
			}
			Vector3 localPosition = a.cubeRoot.get_localPosition();
			Vector3 localPosition2 = b.cubeRoot.get_localPosition();
			if (localPosition.y < localPosition2.y - 0.3f)
			{
				return -1;
			}
			if (localPosition2.y < localPosition.y - 0.3f)
			{
				return 1;
			}
			Vector3 val = localPosition - wheelsCenter;
			float sqrMagnitude = val.get_sqrMagnitude();
			Vector3 val2 = localPosition2 - wheelsCenter;
			float sqrMagnitude2 = val2.get_sqrMagnitude();
			if (sqrMagnitude > sqrMagnitude2 + 0.05f)
			{
				return -1;
			}
			if (sqrMagnitude2 > sqrMagnitude + 0.05f)
			{
				return 1;
			}
			return 0;
		}
	}
}
