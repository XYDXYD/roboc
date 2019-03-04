using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class MachineEditorCollisionChecker : IPhysicallyTickable, IInitialize, ITickableBase
	{
		private Queue<GameObject> _cubesToCheck = new Queue<GameObject>();

		private GameObject _currentlyChecking;

		private int _fixedFrameCubeCheckCounter;

		private Dictionary<int, HashSet<int>> _collisionTargets = new Dictionary<int, HashSet<int>>();

		[Inject]
		internal IMachineBuilder machineBuilder
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColorUpdater colorUpdater
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

		internal bool CheckCubeAndMakeInvalidIfNecessary(GameObject cubeToCheck)
		{
			int instanceID = cubeToCheck.GetInstanceID();
			if (_collisionTargets.ContainsKey(instanceID))
			{
				int count = _collisionTargets[instanceID].Count;
				if (count > 0)
				{
					colorUpdater.MarkAsValid(cubeToCheck, isValid: false);
					return true;
				}
			}
			CubeCollisionCheckerComponent component = cubeToCheck.GetComponent<CubeCollisionCheckerComponent>();
			if (Object.op_Implicit(component))
			{
				component.MoveToVerifiedCollisionLayer();
				RemoveFromCollisionMap(cubeToCheck);
			}
			return false;
		}

		void IInitialize.OnDependenciesInjected()
		{
			machineMap.OnRemoveCubeAt += HandleOnRemoveCubeAt;
		}

		private void HandleOnRemoveCubeAt(Byte3 arg1, MachineCell cell)
		{
			RemoveFromCollisionMap(cell.gameObject);
		}

		private void SetPairCollision(GameObject primaryObject, GameObject secondaryObject)
		{
			int instanceID = primaryObject.GetInstanceID();
			int instanceID2 = secondaryObject.GetInstanceID();
			if (_collisionTargets.ContainsKey(instanceID))
			{
				_collisionTargets[instanceID].Add(instanceID2);
				return;
			}
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(instanceID2);
			_collisionTargets[instanceID] = hashSet;
		}

		private void RemoveFromCollisionMap(GameObject cubeToCheck)
		{
			int instanceID = cubeToCheck.GetInstanceID();
			if (_collisionTargets.ContainsKey(instanceID))
			{
				_collisionTargets.Remove(instanceID);
			}
			foreach (KeyValuePair<int, HashSet<int>> collisionTarget in _collisionTargets)
			{
				if (collisionTarget.Value != null && collisionTarget.Value.Contains(instanceID))
				{
					collisionTarget.Value.Remove(instanceID);
				}
			}
		}

		private void RemovePairCollision(GameObject primaryObject, GameObject secondaryObject)
		{
			int instanceID = primaryObject.GetInstanceID();
			int instanceID2 = secondaryObject.GetInstanceID();
			if (_collisionTargets.ContainsKey(instanceID))
			{
				HashSet<int> hashSet = _collisionTargets[instanceID];
				hashSet.Remove(instanceID2);
				if (hashSet.Count == 0)
				{
					_collisionTargets.Remove(primaryObject.GetInstanceID());
				}
				else
				{
					_collisionTargets[instanceID] = hashSet;
				}
			}
		}

		public void SetCollisionBetween(GameObject cubeCheckingCollision, GameObject collisionTarget, bool setting)
		{
			if (setting)
			{
				SetPairCollision(cubeCheckingCollision, collisionTarget);
				SetPairCollision(collisionTarget, cubeCheckingCollision);
			}
			else
			{
				RemovePairCollision(cubeCheckingCollision, collisionTarget);
				RemovePairCollision(collisionTarget, cubeCheckingCollision);
			}
		}

		public void EnqueueCubeToCheck(GameObject cube)
		{
			_cubesToCheck.Enqueue(cube);
		}

		public void PhysicsTick(float deltaTime)
		{
			if (_cubesToCheck.Count > 0 && _currentlyChecking == null && _fixedFrameCubeCheckCounter == 0)
			{
				_currentlyChecking = _cubesToCheck.Dequeue();
				if (_currentlyChecking != null)
				{
					_fixedFrameCubeCheckCounter = 2;
				}
				return;
			}
			if (_fixedFrameCubeCheckCounter == 0 && _currentlyChecking != null)
			{
				if (CheckCubeAndMakeInvalidIfNecessary(_currentlyChecking))
				{
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strIncorrectCubePlacementTitle"), StringTableBase<StringTable>.Instance.GetString("strIncorrectCubePlacementBody")));
					HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
					foreach (InstantiatedCube item in allInstantiatedCubes)
					{
						GameObject cubeAt = machineMap.GetCubeAt(item.gridPos);
						if (cubeAt == _currentlyChecking)
						{
							machineBuilder.RemoveCube(item);
							break;
						}
					}
				}
				_currentlyChecking = null;
			}
			if (_fixedFrameCubeCheckCounter > 0)
			{
				_fixedFrameCubeCheckCounter = 0;
			}
		}
	}
}
