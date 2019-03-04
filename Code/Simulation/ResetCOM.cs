using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal class ResetCOM : MonoBehaviour
	{
		private bool _initialized;

		private Rigidbody _rb;

		private Vector3 _centerOfMass;

		private Vector3 _inertiaTensor;

		private Quaternion _inertiaTensorRotation;

		public ResetCOM()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			_rb = this.GetComponent<Rigidbody>();
			_rb.set_centerOfMass(Vector3.get_zero());
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SaveInitialValues);
		}

		private IEnumerator SaveInitialValues()
		{
			while (_rb != null && _rb.get_centerOfMass() == Vector3.get_zero())
			{
				yield return null;
			}
			if (_rb != null)
			{
				_centerOfMass = _rb.get_centerOfMass();
				_inertiaTensor = _rb.get_inertiaTensor();
				_inertiaTensorRotation = _rb.get_inertiaTensorRotation();
				_initialized = true;
			}
		}

		private void OnEnable()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (_initialized)
			{
				_rb.set_centerOfMass(_centerOfMass);
				_rb.set_inertiaTensor(_inertiaTensor);
				_rb.set_inertiaTensorRotation(_inertiaTensorRotation);
			}
		}
	}
}
