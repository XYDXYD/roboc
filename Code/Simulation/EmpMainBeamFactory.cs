using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class EmpMainBeamFactory
	{
		private Func<EmpMainBeamBehaviour> _onTargetingLocatorAllocation;

		private GameObject _currentPrefab;

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		public EmpMainBeamPool empMainBeamPool
		{
			get;
			set;
		}

		public EmpMainBeamFactory()
		{
			_onTargetingLocatorAllocation = GenerateNewTargetingLocator;
		}

		public void PreallocateMainBeam(GameObject prefab, int numLocators)
		{
			_currentPrefab = prefab;
			empMainBeamPool.Preallocate(_currentPrefab.get_name(), numLocators, _onTargetingLocatorAllocation);
		}

		public EmpMainBeamBehaviour Build(GameObject prefab, Vector3 position, float animationDuration, Vector3 localScale)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			_currentPrefab = prefab;
			EmpMainBeamBehaviour empMainBeamBehaviour = empMainBeamPool.Use(_currentPrefab.get_name(), _onTargetingLocatorAllocation);
			empMainBeamBehaviour.get_transform().set_position(position);
			empMainBeamBehaviour.get_transform().set_localScale(localScale);
			empMainBeamBehaviour.SetAnimationSpeed(animationDuration);
			empMainBeamBehaviour.SetPool(empMainBeamPool);
			empMainBeamBehaviour.get_gameObject().SetActive(true);
			return empMainBeamBehaviour;
		}

		private EmpMainBeamBehaviour GenerateNewTargetingLocator()
		{
			GameObject val = Object.Instantiate<GameObject>(_currentPrefab);
			val.set_name(_currentPrefab.get_name());
			val.SetActive(false);
			return val.GetComponent<EmpMainBeamBehaviour>();
		}
	}
}
