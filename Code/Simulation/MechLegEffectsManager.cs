using UnityEngine;

namespace Simulation
{
	internal sealed class MechLegEffectsManager
	{
		private Vector3 _effectsOffset = new Vector3(0f, 0.1f, 0f);

		public void PlayJumpingEffect(CubeMechLeg leg, Vector3 position)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			PlayEffectLocal(leg, _effectsOffset, MechLegEffects.Effect.JumpingEffect);
			leg.legGraphics.justJumped = false;
		}

		public void PlayLandingEffect(CubeMechLeg leg, Vector3 position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position2 = position + _effectsOffset * 2f;
			PlayEffect(leg, position2, MechLegEffects.Effect.LandingEffect);
			leg.legGraphics.justLanded = false;
		}

		public void PlayWalkingEffect(CubeMechLeg leg, Vector3 position)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			PlayEffectLocal(leg, _effectsOffset, MechLegEffects.Effect.WalkingEffect);
		}

		public void PlayStartLongJumpEffect(CubeMechLeg leg, Vector3 position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position2 = position + _effectsOffset * 2f;
			PlayEffect(leg, position2, MechLegEffects.Effect.StartLongJumpEffect);
		}

		public void PlayLongJumpEffect(CubeMechLeg leg, Vector3 position)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			PlayEffectLocal(leg, Vector3.get_zero(), MechLegEffects.Effect.LongJumpingEffect);
		}

		public void EnableRunningEffect(CubeMechLeg leg, bool enabled)
		{
			MechLegEffects legEffects = leg.legEffects;
			if (legEffects.avalaibleEffects[4])
			{
				Transform val = legEffects.effectsTransform[4];
				GameObject gameObject = val.get_gameObject();
				if (leg.isOffScreen && gameObject.get_activeInHierarchy())
				{
					gameObject.SetActive(false);
				}
				else if (gameObject.get_activeInHierarchy() != enabled)
				{
					gameObject.SetActive(enabled);
				}
			}
		}

		public void PlaySkiddingEffect(CubeMechLeg leg, Vector3 position)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			MechLegEffects legEffects = leg.legEffects;
			MechLegGraphics legGraphics = leg.legGraphics;
			if (legEffects.skiddingEffectHidden)
			{
				PlayEffectLocal(leg, _effectsOffset, MechLegEffects.Effect.SkiddingEffect);
			}
			legEffects.skiddingEffectHidden = false;
			if (!legGraphics.isSkidding)
			{
				legGraphics.isSkidding = true;
			}
		}

		public void HideSkiddingEffect(CubeMechLeg leg)
		{
			HideEffect(leg, MechLegEffects.Effect.SkiddingEffect);
			leg.legEffects.skiddingEffectHidden = true;
			leg.legGraphics.isSkidding = false;
		}

		private void PlayEffectLocal(CubeMechLeg leg, Vector3 offset, MechLegEffects.Effect effectIndex)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (!leg.isOffScreen)
			{
				MechLegEffects legEffects = leg.legEffects;
				legEffects.effectsTransform[(int)effectIndex].set_localPosition(Vector3.get_zero() + offset);
				PlayParticles(effectIndex, legEffects);
			}
		}

		private void PlayEffect(CubeMechLeg leg, Vector3 position, MechLegEffects.Effect effectIndex)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (!leg.isOffScreen)
			{
				MechLegEffects legEffects = leg.legEffects;
				legEffects.effectsTransform[(int)effectIndex].set_parent(null);
				legEffects.effectsTransform[(int)effectIndex].set_position(position);
				Vector3 val = Vector3.Cross(leg.legData.footHitNormal, leg.T.get_right());
				legEffects.effectsTransform[(int)effectIndex].set_rotation(Quaternion.LookRotation(val, leg.legData.footHitNormal));
				PlayParticles(effectIndex, legEffects);
			}
		}

		private void PlayParticles(MechLegEffects.Effect effectIndex, MechLegEffects legEffects)
		{
			for (int i = 0; i < legEffects.particleSystems[(int)effectIndex].Length; i++)
			{
				ParticleSystem val = legEffects.particleSystems[(int)effectIndex][i];
				if (val != null)
				{
					if (val.IsAlive(true))
					{
						val.Stop(true);
						val.Clear(true);
					}
					val.Simulate(0.0001f, true, true);
					val.Play(true);
				}
			}
		}

		private void HideEffect(CubeMechLeg leg, MechLegEffects.Effect effectIndex)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			MechLegEffects legEffects = leg.legEffects;
			ParticleSystem val = null;
			legEffects.effectsTransform[(int)effectIndex].set_localPosition(Vector3.get_zero());
			for (int i = 0; i < legEffects.particleSystems[(int)effectIndex].Length; i++)
			{
				val = legEffects.particleSystems[(int)effectIndex][i];
				if (val != null)
				{
					val.Stop(true);
					val.Clear(true);
				}
			}
		}
	}
}
