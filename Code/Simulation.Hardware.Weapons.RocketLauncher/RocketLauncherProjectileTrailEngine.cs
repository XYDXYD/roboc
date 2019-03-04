using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class RocketLauncherProjectileTrailEngine : SingleEntityViewEngine<RocketLauncherProjectileTrailNode>, ITickable, ITickableBase
	{
		private Dictionary<int, RocketLauncherProjectileTrailNode> _trailNodes = new Dictionary<int, RocketLauncherProjectileTrailNode>(200);

		private FasterList<RocketLauncherProjectileTrailNode> _fadingTrailNodes = new FasterList<RocketLauncherProjectileTrailNode>(100);

		protected override void Add(RocketLauncherProjectileTrailNode obj)
		{
			_trailNodes.Add(obj.get_ID(), obj);
			obj.projectileAliveComponent.resetProjectile.subscribers += FadeTrail;
			obj.trailEffects.trailResetNeeded = true;
			obj.trailEffects.trailRenderer.Clear();
			obj.trailEffects.trailRenderer.set_time(obj.trailEffects.maxTrailTime);
			obj.trailEffects.trailRenderer.get_gameObject().SetActive(true);
		}

		protected override void Remove(RocketLauncherProjectileTrailNode obj)
		{
			_trailNodes.Remove(obj.get_ID());
			obj.projectileAliveComponent.resetProjectile.subscribers -= FadeTrail;
		}

		public void Tick(float deltaSec)
		{
			if (_fadingTrailNodes.get_Count() <= 0)
			{
				return;
			}
			for (int num = _fadingTrailNodes.get_Count() - 1; num >= 0; num--)
			{
				RocketLauncherProjectileTrailNode rocketLauncherProjectileTrailNode = _fadingTrailNodes.get_Item(num);
				if (Time.get_timeSinceLevelLoad() >= rocketLauncherProjectileTrailNode.trailEffects.disableCountdown)
				{
					Reset(rocketLauncherProjectileTrailNode);
					_fadingTrailNodes.UnorderedRemoveAt(num);
				}
			}
		}

		private void FadeTrail(IProjectileAliveComponent sender, int instanceId)
		{
			RocketLauncherProjectileTrailNode rocketLauncherProjectileTrailNode = _trailNodes[instanceId];
			TrailRenderer trailRenderer = rocketLauncherProjectileTrailNode.trailEffects.trailRenderer;
			float num = Time.get_timeSinceLevelLoad() - rocketLauncherProjectileTrailNode.projectileTimeComponent.startTime;
			if (num < rocketLauncherProjectileTrailNode.trailEffects.maxTrailTime)
			{
				trailRenderer.set_time(num);
			}
			rocketLauncherProjectileTrailNode.trailEffects.disableCountdown = trailRenderer.get_time() + Time.get_timeSinceLevelLoad();
			_fadingTrailNodes.Add(rocketLauncherProjectileTrailNode);
			rocketLauncherProjectileTrailNode.trailEffects.projectile.SetActive(false);
		}

		private void Reset(RocketLauncherProjectileTrailNode node)
		{
			node.trailEffects.trailRenderer.Clear();
			node.trailEffects.trailRenderer.set_time(node.trailEffects.maxTrailTime);
			node.transformComponent.T.get_gameObject().SetActive(false);
			node.trailEffects.trailRenderer.get_gameObject().SetActive(true);
			node.trailEffects.projectile.SetActive(true);
		}
	}
}
