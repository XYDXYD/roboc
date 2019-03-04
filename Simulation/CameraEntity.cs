using Simulation.DeathEffects;
using Simulation.Hardware.Weapons;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Simulation
{
	internal sealed class CameraEntity : MonoBehaviour, ICameraControlComponent, IEmpCameraEffectsComponent, IBlinkCameraEffectsComponent, ICameraShakeDamageComponent
	{
		[SerializeField]
		private GlitchEffect empGlitchEffect;

		[SerializeField]
		private ColorCorrectionCurves empColorEffect;

		[SerializeField]
		private GameObject _cameraStunSoundObject;

		[SerializeField]
		private GameObject blinkCameraEffect;

		[SerializeField]
		private VignetteAndChromaticAberration blinkAberrationEffect;

		[SerializeField]
		private float openTime = 0.1f;

		[SerializeField]
		private float closeTime = 0.1f;

		[SerializeField]
		private float startValueVignetting;

		[SerializeField]
		private float endValueVignetting = 0.5f;

		[SerializeField]
		private float startValueAberration;

		[SerializeField]
		private float endValueAberration = 8f;

		[SerializeField]
		private float durationUpTransitionVignetting = 0.1f;

		[SerializeField]
		private float durationDownTransitionVignetting = 0.1f;

		[SerializeField]
		private float durationUpTransitionAberration = 0.1f;

		[SerializeField]
		private float durationDownTransitionAberration = 0.1f;

		private SimulationCamera _controlScript;

		private CameraShake _cameraShakeDamage;

		bool ICameraControlComponent.controlScriptEnabled
		{
			get
			{
				return _controlScript.get_enabled();
			}
			set
			{
				_controlScript.set_enabled(value);
			}
		}

		bool ICameraControlComponent.instantFollowEnabled
		{
			get
			{
				return _controlScript.GetInstantFollow();
			}
			set
			{
				_controlScript.SetInstantFollow(value);
			}
		}

		bool ICameraControlComponent.activateProgressiveFollow
		{
			get
			{
				return _controlScript.GetProgressiveFollow();
			}
			set
			{
				_controlScript.SetProgressiveFollow(value);
			}
		}

		float ICameraControlComponent.cameraTime
		{
			get
			{
				return _controlScript.GetCameraTime();
			}
			set
			{
				_controlScript.SetCameraTime(value);
			}
		}

		Vector3 ICameraControlComponent.lastCameraPosition
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return _controlScript.GetLastCameraPosition();
			}
		}

		Vector3 ICameraControlComponent.finalExpectedCameraPosition
		{
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				_controlScript.SetExpectedCameraPosition(value);
			}
		}

		Transform ICameraShakeDamageComponent.transformToShake
		{
			get
			{
				return _cameraShakeDamage.get_transform();
			}
		}

		int ICameraShakeDamageComponent.maxSimultaneouslyShake
		{
			get
			{
				return _cameraShakeDamage.maxSimultaneouslyShake;
			}
		}

		float ICameraShakeDamageComponent.rotationDamageMagnitudeMultiplier
		{
			get
			{
				return _cameraShakeDamage.rotationDamageMagnitudeMultiplier;
			}
		}

		float ICameraShakeDamageComponent.translationDamageMagnitudeMultiplier
		{
			get
			{
				return _cameraShakeDamage.translationDamageMagnitudeMultiplier;
			}
		}

		float ICameraShakeDamageComponent.damageDuration
		{
			get
			{
				return _cameraShakeDamage.damageDuration;
			}
		}

		TranslationCurve ICameraShakeDamageComponent.translationDamageCurves
		{
			get
			{
				return _cameraShakeDamage.translationDamageCurves;
			}
		}

		RotationCurve ICameraShakeDamageComponent.rotationDamageCurves
		{
			get
			{
				return _cameraShakeDamage.rotationDamageCurves;
			}
		}

		GameObject IBlinkCameraEffectsComponent.blinkCameraEffect
		{
			get
			{
				return blinkCameraEffect;
			}
		}

		float IBlinkCameraEffectsComponent.openTime
		{
			get
			{
				return openTime;
			}
		}

		float IBlinkCameraEffectsComponent.closeTime
		{
			get
			{
				return closeTime;
			}
		}

		float IBlinkCameraEffectsComponent.startValueVignetting
		{
			get
			{
				return startValueVignetting;
			}
		}

		float IBlinkCameraEffectsComponent.endValueVignetting
		{
			get
			{
				return endValueVignetting;
			}
		}

		float IBlinkCameraEffectsComponent.startValueAberration
		{
			get
			{
				return startValueAberration;
			}
		}

		float IBlinkCameraEffectsComponent.endValueAberration
		{
			get
			{
				return endValueAberration;
			}
		}

		float IBlinkCameraEffectsComponent.durationUpTransitionVignetting
		{
			get
			{
				return durationUpTransitionVignetting;
			}
		}

		float IBlinkCameraEffectsComponent.durationDownTransitionVignetting
		{
			get
			{
				return durationDownTransitionVignetting;
			}
		}

		float IBlinkCameraEffectsComponent.durationUpTransitionAberration
		{
			get
			{
				return durationUpTransitionAberration;
			}
		}

		float IBlinkCameraEffectsComponent.durationDownTransitionAberration
		{
			get
			{
				return durationDownTransitionAberration;
			}
		}

		VignetteAndChromaticAberration IBlinkCameraEffectsComponent.blinkAberrationEffect
		{
			get
			{
				return blinkAberrationEffect;
			}
		}

		[Inject]
		public IEntityFactory enginesRoot
		{
			private get;
			set;
		}

		public bool enableEffectsScripts
		{
			get
			{
				return empGlitchEffect.get_enabled() && empColorEffect.get_enabled();
			}
			set
			{
				empGlitchEffect.set_enabled(value);
				empColorEffect.set_enabled(value);
			}
		}

		public GameObject cameraStunSoundObject => _cameraStunSoundObject;

		public float mainCameraEffectTimer
		{
			get;
			set;
		}

		public float vignettingEffectTimer
		{
			get;
			set;
		}

		public float aberrationEffectTimer
		{
			get;
			set;
		}

		public Renderer blinkCameraEffectRenderer
		{
			get;
			private set;
		}

		public CameraEntity()
			: this()
		{
		}

		private void Start()
		{
			empColorEffect.set_enabled(false);
			blinkAberrationEffect.set_enabled(false);
			empGlitchEffect.set_enabled(false);
			int instanceID = this.GetInstanceID();
			DeathAnimationBroadcastImplementor item = new DeathAnimationBroadcastImplementor(instanceID);
			List<object> list = new List<object>(this.GetComponentsInChildren<MonoBehaviour>());
			list.Add(item);
			enginesRoot.BuildEntity<CameraEntityDescriptor>(instanceID, list.ToArray());
			_controlScript = this.GetComponent<SimulationCamera>();
			blinkCameraEffectRenderer = blinkCameraEffect.GetComponentInChildren<Renderer>();
			_cameraShakeDamage = this.GetComponentInChildren<CameraShake>();
		}
	}
}
