using UnityEngine;

namespace Simulation.Hardware.Weapons.Chaingun
{
	public class ChaingunSpinEffectMonoBehaviour : MonoBehaviour, IWeaponSpinVortexEffectComponent, IWeaponSpinTransformComponent, IWeaponHeatEffectComponent, IImplementor
	{
		public Renderer vortexEffectRenderer;

		[Range(0f, 1f)]
		[Tooltip("Spin power over which the spin vortex starts to be visible")]
		public float vortexThreshold;

		public Renderer[] heatEffectRenderers;

		[Tooltip("Speed at which the heat is increasing at full fire rate. Should be higher than the decrease speed to be noticeable.")]
		public float _heatIncreaseSpeed = 1f;

		[Tooltip("Speed at which the heat is constantly decreasing")]
		public float _heatDecreaseSpeed = 0.5f;

		public Transform barrelTransform;

		public float speedScale = 60f;

		public Renderer spinVortexRenderer => vortexEffectRenderer;

		public float spinVortexThreshold => vortexThreshold;

		public Renderer[] heatRenderers => heatEffectRenderers;

		public float heatIncreaseSpeed => _heatIncreaseSpeed;

		public float heatDecreaseSpeed => _heatDecreaseSpeed;

		public float currentHeat
		{
			get;
			set;
		}

		public Transform spinBarrelTransform => barrelTransform;

		public float spinBarrelSpeedScale => speedScale;

		public ChaingunSpinEffectMonoBehaviour()
			: this()
		{
		}
	}
}
