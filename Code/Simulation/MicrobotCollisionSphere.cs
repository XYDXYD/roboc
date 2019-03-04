using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Simulation
{
	internal class MicrobotCollisionSphere
	{
		private int _layer;

		private Transform _parent;

		private float _microbotSphereVolume;

		private float _microbotSphereRadius;

		private SphereCollider _lowVolumeSphereCollider;

		private FasterList<Collider> _nonClusteredColliders;

		private IServiceRequestFactory _serviceFactory;

		private ObjectPool<SphereCollider> _sphereColliderPool;

		private Func<SphereCollider> _generateNewSphereCollider;

		[CompilerGenerated]
		private static Func<SphereCollider> _003C_003Ef__mg_0024cache0;

		public MicrobotCollisionSphere(IServiceRequestFactory serviceFactory, MachineSphereColliderPool sphereColliderPool, FasterList<Collider> nonClusteredColliders)
		{
			_serviceFactory = serviceFactory;
			LoadMicrobotSphereSettings();
			_sphereColliderPool = sphereColliderPool;
			_generateNewSphereCollider = GenerateNewSphereCollider;
			_nonClusteredColliders = new FasterList<Collider>(nonClusteredColliders);
		}

		private void LoadMicrobotSphereSettings()
		{
			IGetGameClientSettingsRequest getGameClientSettingsRequest = _serviceFactory.Create<IGetGameClientSettingsRequest>();
			getGameClientSettingsRequest.SetAnswer(new ServiceAnswer<GameClientSettingsDependency>(delegate(GameClientSettingsDependency data)
			{
				_microbotSphereRadius = data.microbotSphereRadius;
				_microbotSphereVolume = 1.33333337f * ((float)Math.PI * (_microbotSphereRadius * _microbotSphereRadius * _microbotSphereRadius));
			}, delegate(ServiceBehaviour behaviour)
			{
				ErrorWindow.ShowServiceErrorWindow(behaviour);
			}));
			getGameClientSettingsRequest.Execute();
		}

		public void RecycleCollider()
		{
			_sphereColliderPool.Recycle(_lowVolumeSphereCollider, 0);
			_lowVolumeSphereCollider = null;
		}

		private SphereCollider GenerateLowVolumeSphereCollider()
		{
			SphereCollider val = _sphereColliderPool.Use(0, _generateNewSphereCollider);
			val.set_radius(_microbotSphereRadius);
			GameObject gameObject = val.get_gameObject();
			Transform transform = gameObject.get_transform();
			transform.set_parent(_parent);
			gameObject.set_layer(_layer);
			gameObject.SetActive(false);
			return val;
		}

		internal static SphereCollider GenerateNewSphereCollider()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			SphereCollider val = new GameObject("LowVolumeSphereCollider").AddComponent<SphereCollider>();
			val.get_gameObject().set_layer(GameLayers.IGNORE_RAYCAST);
			val.set_tag(WeaponRaycastUtility.MICROBOT_SPHERE_COLLIDER_TAG);
			val.set_isTrigger(true);
			return val;
		}

		public void SetLayer(int layer)
		{
			_layer = layer;
		}

		public void TryActivateSphereCollider(Transform parent, MachineCluster cluster, Vector3 position, FasterList<Collider> newColliders)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			_parent = parent;
			if (_lowVolumeSphereCollider == null)
			{
				_lowVolumeSphereCollider = GenerateLowVolumeSphereCollider();
				newColliders.Add(_lowVolumeSphereCollider);
			}
			_lowVolumeSphereCollider.get_transform().set_localPosition(position);
			_lowVolumeSphereCollider.set_radius(_microbotSphereRadius);
			float num = CalculateMachineVolume(cluster.GetColliders());
			ActivateSphereCollider(num < _microbotSphereVolume);
		}

		internal SphereCollider GetSphereCollider()
		{
			return _lowVolumeSphereCollider;
		}

		internal FasterList<Collider> GetNonClusteredColliders()
		{
			return _nonClusteredColliders;
		}

		private float CalculateMachineVolume(FasterList<NodeBoxCollider> colliders)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			for (int i = 0; i < colliders.get_Count(); i++)
			{
				NodeBoxCollider nodeBoxCollider = colliders.get_Item(i);
				Vector3 size = nodeBoxCollider.collider.get_size();
				float num2 = size.x * size.y * size.z;
				num += num2;
			}
			for (int j = 0; j < _nonClusteredColliders.get_Count(); j++)
			{
				Collider val = _nonClusteredColliders.get_Item(j);
				if (val.get_gameObject().get_activeInHierarchy() && val.get_enabled())
				{
					if (val is BoxCollider)
					{
						Vector3 size2 = (val as BoxCollider).get_size();
						float num3 = size2.x * size2.y * size2.z;
						num += num3;
					}
					else if (val is SphereCollider)
					{
						float radius = (val as SphereCollider).get_radius();
						float num4 = 1.33333337f * ((float)Math.PI * (radius * radius * radius));
						num += num4;
					}
				}
			}
			return num;
		}

		private void ActivateSphereCollider(bool activate)
		{
			GameObject gameObject = _lowVolumeSphereCollider.get_gameObject();
			if (gameObject.get_activeInHierarchy() != activate)
			{
				gameObject.SetActive(activate);
			}
		}
	}
}
