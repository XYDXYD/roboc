using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class CrackDecalProjectorFactory
	{
		private Func<CrackDecalProjectorAutoRecycleBehaviour> _onTargetingLocatorAllocation;

		private GameObject _currentPrefab;

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		public CrackDecalProjectorPool decalPool
		{
			get;
			set;
		}

		public CrackDecalProjectorFactory()
		{
			_onTargetingLocatorAllocation = GenerateNewTargetingLocator;
		}

		public void PreallocateCrackDecal(GameObject prefab, int numDecals)
		{
			_currentPrefab = prefab;
			decalPool.Preallocate(prefab.get_name(), numDecals, _onTargetingLocatorAllocation);
		}

		public CrackDecalProjectorAutoRecycleBehaviour Build(GameObject prefab, Vector3 position, Vector3 localScale, float stunDuration)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			_currentPrefab = prefab;
			CrackDecalProjectorAutoRecycleBehaviour crackDecalProjectorAutoRecycleBehaviour = decalPool.Use(prefab.get_name(), _onTargetingLocatorAllocation);
			float y = position.y;
			Vector3 position2 = crackDecalProjectorAutoRecycleBehaviour.get_transform().get_position();
			position.y = y + position2.y;
			crackDecalProjectorAutoRecycleBehaviour.get_transform().set_position(position);
			crackDecalProjectorAutoRecycleBehaviour.SetBaseDuration(stunDuration);
			crackDecalProjectorAutoRecycleBehaviour.get_gameObject().SetActive(true);
			crackDecalProjectorAutoRecycleBehaviour.get_transform().set_localScale(localScale);
			return crackDecalProjectorAutoRecycleBehaviour;
		}

		private CrackDecalProjectorAutoRecycleBehaviour GenerateNewTargetingLocator()
		{
			GameObject val = gameObjectFactory.Build(_currentPrefab);
			val.set_name(_currentPrefab.get_name());
			val.SetActive(false);
			return val.GetComponent<CrackDecalProjectorAutoRecycleBehaviour>();
		}
	}
}
