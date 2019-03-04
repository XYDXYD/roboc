using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class GenericProjectileTrailEngine : SingleEntityViewEngine<GenericProjectileTrailNode>, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private Action<IProjectileAliveComponent, int> delegateToUse;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public GenericProjectileTrailEngine()
		{
			delegateToUse = ResetProjectileTrail;
		}

		public void Ready()
		{
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<GenericProjectileTrailNode> val = entityViewsDB.QueryEntityViews<GenericProjectileTrailNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				CheckIfDisabled(val.get_Item(i), deltaSec);
			}
		}

		protected override void Add(GenericProjectileTrailNode node)
		{
			IProjectileAliveComponent projectileAliveComponent = node.projectileAliveComponent;
			projectileAliveComponent.resetProjectile.subscribers += delegateToUse;
			node.TrailEffectsComponent.projectileTrail.Clear();
			node.TrailEffectsComponent.projectileTrail.get_gameObject().SetActive(true);
		}

		protected override void Remove(GenericProjectileTrailNode node)
		{
			node.projectileAliveComponent.resetProjectile.subscribers -= delegateToUse;
		}

		private void ResetProjectileTrail(IProjectileAliveComponent sender, int entityId)
		{
			GenericProjectileTrailNode node = entityViewsDB.QueryEntityView<GenericProjectileTrailNode>(entityId);
			FadeProjectile(node);
		}

		private void FadeProjectile(GenericProjectileTrailNode node)
		{
			IProjectileTrailEffectsComponent trailEffectsComponent = node.TrailEffectsComponent;
			float num = trailEffectsComponent.disableCountdown = trailEffectsComponent.projectileTrail.get_time();
			SwitchRenderersOnHit(node, enabled: false);
		}

		private void ResetProjectile(GenericProjectileTrailNode node)
		{
			Transform t = node.transformComponent.T;
			node.TrailEffectsComponent.projectileTrail.Clear();
			t.get_gameObject().SetActive(false);
			node.TrailEffectsComponent.trailRendererObject.SetActive(true);
			SwitchRenderersOnHit(node, enabled: true);
		}

		private static void SwitchRenderersOnHit(GenericProjectileTrailNode node, bool enabled)
		{
			if (node.TrailEffectsComponent.disableOnHit != null)
			{
				node.TrailEffectsComponent.disableOnHit.SetActive(enabled);
			}
		}

		private void CheckIfDisabled(GenericProjectileTrailNode node, float deltaTime)
		{
			IProjectileTrailEffectsComponent trailEffectsComponent = node.TrailEffectsComponent;
			if (trailEffectsComponent.disableCountdown > 0f)
			{
				trailEffectsComponent.disableCountdown -= deltaTime;
				if (trailEffectsComponent.disableCountdown <= 0f)
				{
					ResetProjectile(node);
				}
			}
		}
	}
}
