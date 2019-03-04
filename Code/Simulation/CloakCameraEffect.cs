using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Simulation
{
	internal sealed class CloakCameraEffect : MonoBehaviour
	{
		public GameObject cloakCameraEffect;

		public Renderer cloakEffectMaterial;

		public float openTime = 0.1f;

		public float closeTime = 0.1f;

		public float startValueVignetting;

		public float endValueVignetting = 0.5f;

		public float startValueAberration;

		public float endValueAberration = 8f;

		public float durationUpTransitionVignetting = 0.1f;

		public float durationDownTransitionVignetting = 0.1f;

		public float durationUpTransitionAberration = 0.1f;

		public float durationDownTransitionAberration = 0.1f;

		public CloakCameraEffect()
			: this()
		{
		}

		private void Start()
		{
			cloakEffectMaterial = cloakCameraEffect.GetComponentInChildren<Renderer>();
			cloakEffectMaterial.get_material().SetFloat("_Range", 0f);
			VignetteAndChromaticAberration component = this.GetComponent<VignetteAndChromaticAberration>();
			component.intensity = startValueVignetting;
			component.chromaticAberration = startValueAberration;
		}
	}
}
