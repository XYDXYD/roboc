using Svelto.ECS;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ProjectileFactory
	{
		private readonly Func<MonoBehaviour> _onProjectileAllocation;

		private GameObject singleThreadedCurrentGameObject;

		[Inject(name = "ProjectileFactory")]
		public MonoBehaviourPool projectilePool
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

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			get;
			private set;
		}

		public ProjectileFactory()
		{
			_onProjectileAllocation = GenerateNewProjectile;
		}

		public BaseProjectileMonoBehaviour Build(WeaponShootingNode weapon, ref Vector3 launchPosition, ref Vector3 direction, ref bool isLocal)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			singleThreadedCurrentGameObject = ((!weapon.weaponOwner.isEnemy) ? weapon.projectilePrefabComponent.projectilePrefab : weapon.projectilePrefabComponent.projectilePrefabEnemy);
			BaseProjectileMonoBehaviour baseProjectileMonoBehaviour = projectilePool.Use<BaseProjectileMonoBehaviour>(singleThreadedCurrentGameObject.get_name(), _onProjectileAllocation);
			baseProjectileMonoBehaviour.get_gameObject().SetActive(true);
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, weapon.weaponOwner.machineId);
			baseProjectileMonoBehaviour.SetGenericProjectileParamaters(weapon, launchPosition, direction, rigidBodyData.get_position(), isLocal);
			return baseProjectileMonoBehaviour;
		}

		public void PreallocateProjectiles(GameObject prefabObject, ItemCategory itemType)
		{
			singleThreadedCurrentGameObject = prefabObject;
			switch (itemType)
			{
			case ItemCategory.Tesla:
				break;
			case ItemCategory.Laser:
			case ItemCategory.Plasma:
			case ItemCategory.Mortar:
			case ItemCategory.Rail:
			case ItemCategory.Nano:
			case ItemCategory.Aeroflak:
			case ItemCategory.Ion:
			case ItemCategory.Chaingun:
				projectilePool.Preallocate(singleThreadedCurrentGameObject.get_name(), 2, _onProjectileAllocation);
				break;
			case ItemCategory.Seeker:
				projectilePool.Preallocate(singleThreadedCurrentGameObject.get_name(), 10, _onProjectileAllocation);
				break;
			default:
				throw new ArgumentOutOfRangeException("itemType", itemType, null);
			}
		}

		private MonoBehaviour GenerateNewProjectile()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			MonoBehaviour val = projectilePool.AddRecycleOnDisableForComponent<MonoBehaviour, ProjectileAutoRecycleBehaviour>(singleThreadedCurrentGameObject);
			GameObject gameObject = val.get_gameObject();
			gameObject.get_transform().set_position(new Vector3(0f, 1000f, 0f));
			engineRoot.BuildEntity(gameObject.GetInstanceID(), gameObject.GetComponent<IEntityDescriptorHolder>().RetrieveDescriptor(), (object[])gameObject.GetComponentsInChildren<MonoBehaviour>());
			return val;
		}
	}
}
