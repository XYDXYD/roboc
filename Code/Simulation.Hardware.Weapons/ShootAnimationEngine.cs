using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class ShootAnimationEngine : SingleEntityViewEngine<ShootAnimationNode>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(ShootAnimationNode node)
		{
			node.projectileCreationComponent.createProjectile.subscribers += OnProjectileFired;
		}

		protected override void Remove(ShootAnimationNode node)
		{
			node.projectileCreationComponent.createProjectile.subscribers -= OnProjectileFired;
		}

		private void OnProjectileFired(IProjectileCreationComponent projectileCreationComponent, ProjectileCreationParams projectileCreationParams)
		{
			int weaponId = projectileCreationParams.weaponId;
			ShootAnimationNode shootAnimationNode = default(ShootAnimationNode);
			if (entityViewsDB.TryQueryEntityView<ShootAnimationNode>(weaponId, ref shootAnimationNode))
			{
				IWeaponAnimationComponent animationComponent = shootAnimationNode.animationComponent;
				animationComponent.animator.Play(animationComponent.shootAnimationName, -1, 0f);
			}
		}
	}
}
