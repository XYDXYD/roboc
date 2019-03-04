using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.Tasks;
using Svelto.Ticker.Legacy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class PhysicsStatusSetter
	{
		private float _angularDrag;

		private PhysicsStatusCalculator _calculator;

		private ITaskRoutine _task;

		private Vector3 _centerOfMass;

		private bool _computeInertiaTensor;

		private bool _dataCopyPending;

		private float _drag;

		private bool _hasBeenChanged;

		private Vector3 _inertiaTensor;

		private Quaternion _inertiaTensorRotation;

		private IMachineMap _machineMap;

		private FasterList<InstantiatedCube> _pendingCalculationCubes;

		private float _prevMass;

		private Rigidbody _rb;

		private object _rigidbodyDataMutex = new object();

		private volatile bool _threadIsAlive;

		private ITicker _ticker;

		public PhysicsStatusSetter(Rigidbody rb, IMachineMap map, bool computeInertiaTensor, TargetType targetType)
		{
			int numberCubes = map.GetNumberCubes();
			_rb = rb;
			_machineMap = map;
			_computeInertiaTensor = computeInertiaTensor;
			_pendingCalculationCubes = new FasterList<InstantiatedCube>(numberCubes);
			_calculator = new PhysicsStatusCalculator(CacheInertiaValues, numberCubes, targetType);
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			_task.SetEnumeratorProvider((Func<IEnumerator>)CalculatePhysicsStatus);
			_task.SetScheduler(StandardSchedulers.get_multiThreadScheduler());
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_physicScheduler(), PhysicsTick());
		}

		private IEnumerator PhysicsTick()
		{
			while (!(_rb == null))
			{
				CopyCalculatedRigidbodyData();
				CheckForRigidbodyChange();
				yield return null;
			}
		}

		private void CacheInertiaValues(PhysicsStatusCalculator.MachineValues results)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			lock (_rigidbodyDataMutex)
			{
				_centerOfMass = results.centerOfMass;
				_inertiaTensor = results.inertiaTensor;
				_inertiaTensorRotation = results.inertiaTensorRotation;
				_drag = results.drag;
				_angularDrag = results.angularDrag;
				_dataCopyPending = true;
			}
		}

		private IEnumerator CalculatePhysicsStatus()
		{
			_threadIsAlive = true;
			Thread.MemoryBarrier();
			try
			{
				_calculator.CalculatePhysicsValues(_pendingCalculationCubes, _computeInertiaTensor);
			}
			catch (Exception ex)
			{
				Console.LogError($"Error while calculating physics status. Message: {ex.Message}. StackTrace: {ex.StackTrace}");
			}
			finally
			{
				_threadIsAlive = false;
				Thread.MemoryBarrier();
			}
			yield return null;
		}

		private void CalculateRigidbodyStatus()
		{
			HashSet<InstantiatedCube> remainingCubes = _machineMap.GetRemainingCubes();
			_pendingCalculationCubes.FastClear();
			_pendingCalculationCubes.AddRange((ICollection<InstantiatedCube>)remainingCubes);
			_task.Start((Action<PausableTaskException>)null, (Action)null);
		}

		private void CheckForRigidbodyChange()
		{
			_hasBeenChanged |= !Mathf.Approximately(_rb.get_mass(), _prevMass);
			Thread.MemoryBarrier();
			if (_hasBeenChanged && !_threadIsAlive)
			{
				CalculateRigidbodyStatus();
				_prevMass = _rb.get_mass();
				_hasBeenChanged = false;
			}
		}

		private void CopyCalculatedRigidbodyData()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			lock (_rigidbodyDataMutex)
			{
				if (_dataCopyPending)
				{
					_rb.set_centerOfMass(_centerOfMass);
					_rb.set_inertiaTensor(_inertiaTensor);
					_rb.set_inertiaTensorRotation(_inertiaTensorRotation);
					_rb.set_drag(_drag);
					_rb.set_angularDrag(_angularDrag);
					_dataCopyPending = false;
				}
			}
		}
	}
}
