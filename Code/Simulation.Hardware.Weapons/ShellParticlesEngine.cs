using Svelto.ECS;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class ShellParticlesEngine : SingleEntityViewEngine<ShellParticlesNode>, IQueryingEntityViewEngine, IEngine
	{
		private NetworkWeaponFiredObserver _networkWeaponFiredObserver;

		private GameObject _currentPrefab;

		private readonly Func<GameObject> _allocateNewShell;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public ShellParticlesEngine()
		{
			_allocateNewShell = AllocateNewShell;
		}

		public void Ready()
		{
		}

		private void HandleOnWeaponFire(IShootingComponent sender, int weaponId)
		{
			ShellParticlesNode node = default(ShellParticlesNode);
			if (entityViewsDB.TryQueryEntityView<ShellParticlesNode>(weaponId, ref node))
			{
				EmitShell(node);
			}
		}

		private void EmitShell(ShellParticlesNode node)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			IWeaponShellParticlesComponent particlesComponent = node.particlesComponent;
			Transform val = particlesComponent.shellParticlesLocations[particlesComponent.nextShellLocation];
			if (++particlesComponent.nextShellLocation == particlesComponent.shellParticlesLocations.Length)
			{
				particlesComponent.nextShellLocation = 0;
			}
			_currentPrefab = particlesComponent.shellParticlesPrefab;
			GameObject val2 = gameObjectPool.Use(_currentPrefab.get_name(), _allocateNewShell);
			_currentPrefab = null;
			Transform transform = val2.get_transform();
			transform.set_parent(val);
			transform.set_position(val.get_position());
			transform.set_rotation(val.get_rotation());
			transform.set_localScale(val.get_localScale());
			val2.SetActive(true);
		}

		private GameObject AllocateNewShell()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentPrefab);
		}

		protected override void Add(ShellParticlesNode node)
		{
			if (node.weaponOwner.ownedByMe)
			{
				node.shootingComponent.shotIsGoingToBeFired.subscribers += HandleOnWeaponFire;
				_currentPrefab = node.particlesComponent.shellParticlesPrefab;
				gameObjectPool.Preallocate(_currentPrefab.get_name(), 4, _allocateNewShell);
				_currentPrefab = null;
			}
		}

		protected override void Remove(ShellParticlesNode node)
		{
			node.shootingComponent.shotIsGoingToBeFired.subscribers -= HandleOnWeaponFire;
		}
	}
}
