using Fabric;
using Svelto.ECS;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class RadarFeedbackEngine : SingleEntityViewEngine<RadarModuleFeedbackNode>, IQueryingEntityViewEngine, IEngine
	{
		private readonly Func<GameObject> _onActivationEffectAllocation;

		private GameObject _currentActivationPrefab;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RadarFeedbackEngine()
		{
			_onActivationEffectAllocation = OnActivationEffectAllocation;
		}

		public void Ready()
		{
		}

		protected override void Add(RadarModuleFeedbackNode node)
		{
			node.radarComponent.isRadarActive.NotifyOnValueSet((Action<int, bool>)OnRadarActivationChanged);
			PreallocateEffect(node);
		}

		protected override void Remove(RadarModuleFeedbackNode node)
		{
			node.radarComponent.isRadarActive.StopNotify((Action<int, bool>)OnRadarActivationChanged);
		}

		private void PreallocateEffect(RadarModuleFeedbackNode node)
		{
			_currentActivationPrefab = node.vfxComponent.activationVfxPrefab;
			gameObjectPool.Preallocate(_currentActivationPrefab.get_name(), 1, _onActivationEffectAllocation);
		}

		private void OnRadarActivationChanged(int moduleId, bool isActive)
		{
			RadarModuleFeedbackNode radarModuleFeedbackNode = entityViewsDB.QueryEntityView<RadarModuleFeedbackNode>(moduleId);
			if (radarModuleFeedbackNode.ownerComponent.ownedByMe && isActive)
			{
				EventManager.get_Instance().PostEvent("Radar_Player_Spotted", 0);
			}
			EventManager.get_Instance().PostEvent((!isActive) ? "Radar_End" : "Radar_Start", 0, (object)null, radarModuleFeedbackNode.vfxComponent.vfxAnchor.get_gameObject());
			if (isActive)
			{
				PlayVFX(radarModuleFeedbackNode);
				radarModuleFeedbackNode.vfxComponent.animatorComponent.Play("radar_startup", -1, 0f);
			}
		}

		private void PlayVFX(RadarModuleFeedbackNode node)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = (!node.ownerComponent.isEnemy) ? node.vfxComponent.activationVfxPrefab : node.vfxComponent.enemyActivationVfxPrefab;
			if (val != null)
			{
				GameObject val2 = CreateActivationEffect(val);
				Transform transform = val2.get_transform();
				transform.set_parent(node.vfxComponent.vfxAnchor);
				transform.set_localPosition(new Vector3(0f, 0f, 0f));
				transform.set_localRotation(Quaternion.get_identity());
				val2.SetActive(true);
			}
		}

		private GameObject CreateActivationEffect(GameObject prefab)
		{
			_currentActivationPrefab = prefab;
			return gameObjectPool.Use(_currentActivationPrefab.get_name(), _onActivationEffectAllocation);
		}

		private GameObject OnActivationEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentActivationPrefab);
		}
	}
}
