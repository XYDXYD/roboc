using System;
using UnityEngine;

namespace Simulation
{
	[Serializable]
	internal sealed class MechLegEffects
	{
		public enum Effect
		{
			WalkingEffect,
			JumpingEffect,
			LandingEffect,
			SkiddingEffect,
			RunningEffect,
			LongJumpingEffect,
			StartLongJumpEffect,
			count
		}

		public GameObject jumpingPrefab;

		public GameObject landingPrefab;

		public GameObject walkingPrefab;

		public GameObject skiddingPrefab;

		public GameObject runningPrefab;

		public GameObject longJumpPrefab;

		public GameObject startLongJumpPrefab;

		[NonSerialized]
		public GameObject[] effects = (GameObject[])new GameObject[7];

		[NonSerialized]
		public Transform[] effectsTransform = (Transform[])new Transform[7];

		[NonSerialized]
		public ParticleSystem[][] particleSystems = new ParticleSystem[7][];

		[NonSerialized]
		public bool skiddingEffectHidden = true;

		[NonSerialized]
		public bool[] avalaibleEffects = new bool[7];

		public void InitialiseLegEffects(Transform parent, Transform altParent)
		{
			InitialiseEffect(walkingPrefab, parent, 0);
			InitialiseEffect(jumpingPrefab, parent, 1);
			InitialiseEffect(landingPrefab, parent, 2);
			InitialiseEffect(skiddingPrefab, parent, 3);
			InitialiseEffect(runningPrefab, parent, 4);
			InitialiseEffect(longJumpPrefab, altParent, 5);
			InitialiseEffect(startLongJumpPrefab, parent, 6);
		}

		private void InitialiseEffect(GameObject prefab, Transform parent, int effectIndex)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			if (prefab != null)
			{
				effects[effectIndex] = Object.Instantiate<GameObject>(prefab);
				effectsTransform[effectIndex] = effects[effectIndex].GetComponent<Transform>();
				effectsTransform[effectIndex].set_parent(parent);
				effectsTransform[effectIndex].set_localPosition(Vector3.get_zero());
				effectsTransform[effectIndex].set_localScale(Vector3.get_one());
				particleSystems[effectIndex] = effects[effectIndex].GetComponentsInChildren<ParticleSystem>();
				avalaibleEffects[effectIndex] = true;
				ParticleSystem[] array = particleSystems[effectIndex];
				foreach (ParticleSystem val in array)
				{
					if (val != null)
					{
						val.Stop(true);
						val.Clear(true);
					}
				}
			}
			else
			{
				avalaibleEffects[effectIndex] = false;
			}
		}
	}
}
