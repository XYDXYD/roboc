using Svelto.ECS;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal sealed class RailProjectileTrailEngine : SingleEntityViewEngine<RailProjectileTrailNode>, IPhysicallyTickable, ITickableBase
	{
		private Dictionary<int, RailProjectileTrailNode> trailNodes = new Dictionary<int, RailProjectileTrailNode>();

		protected override void Add(RailProjectileTrailNode obj)
		{
			trailNodes.Add(obj.get_ID(), obj);
			obj.projectileAliveComponent.resetProjectile.subscribers += ResetProjectileTrail;
			obj.trailEffects.projectileTrail.Clear();
			obj.trailEffects.projectileTrail.get_gameObject().SetActive(true);
		}

		protected override void Remove(RailProjectileTrailNode obj)
		{
			trailNodes.Remove(obj.get_ID());
			obj.projectileAliveComponent.resetProjectile.subscribers -= ResetProjectileTrail;
		}

		public void PhysicsTick(float deltaSec)
		{
			Dictionary<int, RailProjectileTrailNode>.Enumerator enumerator = trailNodes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				RailProjectileTrailNode value = enumerator.Current.Value;
				if (value != null && value.transformComponent.T.get_gameObject().get_activeInHierarchy())
				{
					UpdateSmokeScale(value);
					UpdateSmokeMaterialScale(value);
					UpdateSmokeRotation(value, deltaSec);
					UpdateSmokeDiameter(value, deltaSec);
					FadeSmoke(value, deltaSec);
				}
			}
		}

		private void UpdateSmokeScale(RailProjectileTrailNode node)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			IRailProjectileTrailComponent trailEffects = node.trailEffects;
			IProjectileMovementStatsComponent movementComponent = node.movementComponent;
			Transform t = node.transformComponent.T;
			Vector3 startPosition = movementComponent.startPosition;
			Vector3 position = t.get_position();
			float z = Vector3.Distance(startPosition, position);
			Vector3 localScale = trailEffects.smoke.get_localScale();
			localScale.z = z;
			trailEffects.smoke.set_localScale(localScale);
		}

		private void UpdateSmokeRotation(RailProjectileTrailNode node, float deltaTime)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Transform smoke = node.trailEffects.smoke;
			Quaternion localRotation = smoke.get_localRotation();
			Vector3 eulerAngles = localRotation.get_eulerAngles();
			eulerAngles.z += node.trailEffects.smokeRotateRate * deltaTime;
			localRotation.set_eulerAngles(eulerAngles);
			smoke.set_localRotation(localRotation);
		}

		private void UpdateSmokeDiameter(RailProjectileTrailNode node, float deltaTime)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			IRailProjectileTrailComponent trailEffects = node.trailEffects;
			Vector3 localScale = trailEffects.smoke.get_localScale();
			localScale.x += node.trailEffects.smokeDiameterScaleRate * deltaTime;
			localScale.y += node.trailEffects.smokeDiameterScaleRate * deltaTime;
			trailEffects.smoke.set_localScale(localScale);
		}

		private void UpdateSmokeMaterialScale(RailProjectileTrailNode node)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			IRailProjectileTrailComponent trailEffects = node.trailEffects;
			IProjectileMovementStatsComponent movementComponent = node.movementComponent;
			Transform t = node.transformComponent.T;
			Vector3 startPosition = movementComponent.startPosition;
			Vector3 position = t.get_position();
			float num = Vector3.Distance(startPosition, position);
			trailEffects.smokeMaterial.SetFloat("_ManualScale", num * trailEffects.tilingScale);
		}

		private void FadeSmoke(RailProjectileTrailNode node, float deltaTime)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			IRailProjectileTrailComponent trailEffects = node.trailEffects;
			float num = Time.get_timeSinceLevelLoad() - node.projectileTimeComponent.startTime;
			if (!trailEffects.allowFadeSmoke || !(num > trailEffects.smokeFadeDelay))
			{
				return;
			}
			if (trailEffects.currentSmokeFadeAmount < 1f)
			{
				IProjectileMovementStatsComponent movementComponent = node.movementComponent;
				Transform t = node.transformComponent.T;
				Vector3 startPosition = movementComponent.startPosition;
				Vector3 position = t.get_position();
				float num2 = Vector3.Distance(startPosition, position);
				float num3 = trailEffects.smokeFadeUnitsPerSecond * deltaTime / num2;
				trailEffects.currentSmokeFadeAmount += num3;
				if (trailEffects.currentSmokeFadeAmount > 1f)
				{
					trailEffects.currentSmokeFadeAmount = 1f;
				}
				trailEffects.smokeMaterial.SetFloat("_DissolveAmount", trailEffects.currentSmokeFadeAmount);
			}
			else
			{
				ResetProjectile(node);
			}
		}

		private void ResetProjectileTrail(IProjectileAliveComponent sender, int entityId)
		{
			DisableProjectile(trailNodes[entityId]);
		}

		private void ResetProjectile(RailProjectileTrailNode node)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			node.trailEffects.projectileTrail.Clear();
			node.transformComponent.T.get_gameObject().SetActive(false);
			node.trailEffects.projectileMesh.SetActive(true);
			node.trailEffects.projectileGlow.SetActive(true);
			node.trailEffects.currentSmokeFadeAmount = 0f;
			node.trailEffects.smokeMaterial.SetFloat("_DissolveAmount", 0f);
			node.trailEffects.smokeMaterial.SetFloat("_ManualScale", 0f);
			node.trailEffects.currentSmokeFadeAmount = 0f;
			node.trailEffects.smoke.set_localScale(node.trailEffects.originalSmokeScale);
			Quaternion localRotation = node.trailEffects.smoke.get_localRotation();
			Vector3 eulerAngles = localRotation.get_eulerAngles();
			eulerAngles.z = 0f;
			localRotation.set_eulerAngles(eulerAngles);
			node.trailEffects.smoke.set_localRotation(localRotation);
			node.trailEffects.projectileTrail.set_time(node.trailEffects.originalBeamCollapseTime);
		}

		private void DisableProjectile(RailProjectileTrailNode node)
		{
			IRailProjectileTrailComponent trailEffects = node.trailEffects;
			trailEffects.projectileMesh.SetActive(false);
			trailEffects.projectileGlow.SetActive(false);
			trailEffects.allowFadeSmoke = true;
			trailEffects.projectileTrail.set_time(trailEffects.originalBeamCollapseTime * trailEffects.beamCollapseTimeMultiplier);
		}
	}
}
