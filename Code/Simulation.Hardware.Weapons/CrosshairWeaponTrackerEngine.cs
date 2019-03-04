using Simulation.Hardware.Weapons.Nano;
using Svelto.ECS;
using Svelto.IoC;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CrosshairWeaponTrackerEngine : MultiEntityViewsEngine<WeaponShootingNode, ProjectileNode, HealingProjectileImpactNode>
	{
		[Inject]
		internal CrosshairController crosshairController
		{
			private get;
			set;
		}

		protected override void Add(HealingProjectileImpactNode node)
		{
			node.hitSomethingComponent.hitAlly.subscribers += OnHitEnemy;
		}

		protected override void Remove(HealingProjectileImpactNode node)
		{
			node.hitSomethingComponent.hitAlly.subscribers -= OnHitEnemy;
		}

		protected override void Add(WeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByMe)
			{
				RegisterEvents(obj.hitSomethingComponent);
			}
		}

		protected override void Add(ProjectileNode obj)
		{
			if (obj.projectileOwnerComponent.ownedByMe)
			{
				RegisterEvents(obj.hitSomethingComponent);
			}
		}

		protected override void Remove(WeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByMe)
			{
				DeregisterEvents(obj.hitSomethingComponent);
			}
		}

		protected override void Remove(ProjectileNode obj)
		{
			if (obj.projectileOwnerComponent.ownedByMe)
			{
				DeregisterEvents(obj.hitSomethingComponent);
			}
		}

		private void RegisterEvents(IHitSomethingComponent hitComponent)
		{
			hitComponent.hitEnemy.subscribers += OnHitEnemy;
			hitComponent.hitEnemySplash.subscribers += OnHitEnemy;
			hitComponent.hitProtonium.subscribers += OnHitEnemy;
			hitComponent.hitEqualizer.subscribers += OnHitEnemy;
		}

		private void DeregisterEvents(IHitSomethingComponent hitComponent)
		{
			hitComponent.hitEnemy.subscribers -= OnHitEnemy;
			hitComponent.hitEnemySplash.subscribers -= OnHitEnemy;
			hitComponent.hitProtonium.subscribers -= OnHitEnemy;
			hitComponent.hitEqualizer.subscribers -= OnHitEnemy;
		}

		private void OnHitEnemy(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			if (!hitInfo.targetIsMe)
			{
				crosshairController.ShowDamageEffect(hitInfo.stackCountPercent);
			}
		}
	}
}
