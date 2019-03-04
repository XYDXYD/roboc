using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class RecycleProjectileOnResetEngine : SingleEntityViewEngine<RecycleProjectileOnResetNode>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(RecycleProjectileOnResetNode node)
		{
			node.projectileAliveComponent.resetProjectile.subscribers += HandleResetProjectile;
		}

		protected override void Remove(RecycleProjectileOnResetNode node)
		{
			node.projectileAliveComponent.resetProjectile.subscribers -= HandleResetProjectile;
		}

		private void HandleResetProjectile(IProjectileAliveComponent sender, int projectileId)
		{
			RecycleProjectileOnResetNode recycleProjectileOnResetNode = entityViewsDB.QueryEntityView<RecycleProjectileOnResetNode>(projectileId);
			if (recycleProjectileOnResetNode.transformComponent.T.get_gameObject().get_activeInHierarchy())
			{
				recycleProjectileOnResetNode.transformComponent.T.get_gameObject().SetActive(false);
			}
		}
	}
}
