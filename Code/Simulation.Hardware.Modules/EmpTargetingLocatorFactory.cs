using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class EmpTargetingLocatorFactory
	{
		private Func<EmpTargetingLocatorMonoBehaviour> _onTargetingLocatorAllocation;

		private string _currentPrefabName;

		private const float INITIAL_SIZE = 17f;

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		public EmpTargetingLocatorPool empLocatorPool
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

		public EmpTargetingLocatorFactory()
		{
			_onTargetingLocatorAllocation = GenerateNewTargetingLocator;
		}

		public void PreallocateTargetingLocator(string prefabName, int numLocators)
		{
			_currentPrefabName = prefabName;
			empLocatorPool.Preallocate(_currentPrefabName, numLocators, _onTargetingLocatorAllocation);
		}

		public EmpTargetingLocatorMonoBehaviour Build(string prefabName, Vector3 position, EmpModuleActivationNode empNode, bool isOnMyTeam)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			_currentPrefabName = prefabName;
			EmpTargetingLocatorMonoBehaviour empTargetingLocatorMonoBehaviour = empLocatorPool.Use(prefabName, _onTargetingLocatorAllocation);
			empTargetingLocatorMonoBehaviour.get_transform().set_position(position);
			empTargetingLocatorMonoBehaviour.get_gameObject().SetActive(true);
			empTargetingLocatorMonoBehaviour.SetOwner(empNode.ownerComponent.ownerId, empNode.ownerComponent.machineId, isOnMyTeam);
			empTargetingLocatorMonoBehaviour.SetTimeVariables(empNode.countdownComponent.countdown, empNode.durationComponent.stunDuration);
			float stunRadius = empNode.stunRadiusComponent.stunRadius;
			empTargetingLocatorMonoBehaviour.SetRange(stunRadius);
			Vector3 localScale = empTargetingLocatorMonoBehaviour.get_transform().get_localScale();
			float num = 17f * localScale.x;
			if (Math.Abs(17f - num) > 0.5f)
			{
				float num2 = stunRadius / num;
				Vector3 localScale2 = empTargetingLocatorMonoBehaviour.get_transform().get_localScale();
				empTargetingLocatorMonoBehaviour.get_transform().set_localScale(Vector3.get_right() * localScale2.x * num2 + Vector3.get_up() * localScale2.y + Vector3.get_forward() * localScale2.z * num2);
			}
			return empTargetingLocatorMonoBehaviour;
		}

		private EmpTargetingLocatorMonoBehaviour GenerateNewTargetingLocator()
		{
			GameObject val = gameObjectFactory.Build(_currentPrefabName);
			val.set_name(_currentPrefabName);
			engineRoot.BuildEntity<EmpTargetingLocatorEntityDescriptor>(val.GetInstanceID(), (object[])val.GetComponents<MonoBehaviour>());
			val.SetActive(false);
			return val.GetComponent<EmpTargetingLocatorMonoBehaviour>();
		}
	}
}
