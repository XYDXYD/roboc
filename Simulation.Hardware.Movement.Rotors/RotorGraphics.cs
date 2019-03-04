using System;
using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	[Serializable]
	internal class RotorGraphics
	{
		public GameObject groundEffectPrefab;

		public float effectDistance = 10f;

		public Vector3 effectRotation = new Vector3(-90f, 0f, 0f);

		public float tiltAmount = 15f;

		public float tiltRate = 15f;

		[NonSerialized]
		public Quaternion currentTilt;

		public float rotateRatePerFrame = 20f;

		public float rotateRateFastPerFrame = 40f;

		[NonSerialized]
		public float rotateRandomScale;

		[NonSerialized]
		public Transform groundEffect;

		[NonSerialized]
		public ParticleSystem[] particleSystems;

		[NonSerialized]
		public bool groundEffectHidden;

		public void InitialiseRotorGraphics(Transform rotorCube)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (groundEffectPrefab != null)
			{
				GameObject val = Object.Instantiate<GameObject>(groundEffectPrefab);
				groundEffect = val.GetComponent<Transform>();
				groundEffect.set_parent(rotorCube);
				groundEffect.set_localScale(Vector3.get_one());
				particleSystems = groundEffect.GetComponentsInChildren<ParticleSystem>();
			}
			rotateRandomScale = Random.Range(0.9f, 1.11f);
		}
	}
}
