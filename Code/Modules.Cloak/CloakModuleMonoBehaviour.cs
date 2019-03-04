using Simulation;
using Svelto.ECS;
using UnityEngine;

namespace Modules.Cloak
{
	internal class CloakModuleMonoBehaviour : MonoBehaviour, IReadyEffectActivationComponent, ICloakMaterialsComponent, ICloakAudioObjectsComponent
	{
		[SerializeField]
		private Material fadeToMaterial;

		[SerializeField]
		private Material fadeToMaterialRemote;

		[SerializeField]
		private Material fadeToMaterialRemoteLow;

		[SerializeField]
		private Shader skinnedShader;

		[SerializeField]
		private Shader nonSkinnedShader;

		[SerializeField]
		[Tooltip("Low quality material will be used when the quality setting is smaller or equal to this threshold")]
		private int lowQualityThreshold;

		[SerializeField]
		private GameObject soundObjectActive;

		[SerializeField]
		private GameObject soundObjectInactive;

		private DispatchOnChange<bool> _activateReadyEffect;

		GameObject ICloakAudioObjectsComponent.soundObjectActive
		{
			get
			{
				return soundObjectActive;
			}
		}

		GameObject ICloakAudioObjectsComponent.soundObjectInactive
		{
			get
			{
				return soundObjectInactive;
			}
		}

		Material ICloakMaterialsComponent.fadeToMaterial
		{
			get
			{
				return fadeToMaterial;
			}
		}

		Material ICloakMaterialsComponent.fadeToMaterialRemote
		{
			get
			{
				return fadeToMaterialRemote;
			}
		}

		Material ICloakMaterialsComponent.fadeToMaterialRemoteLow
		{
			get
			{
				return fadeToMaterialRemoteLow;
			}
		}

		int ICloakMaterialsComponent.lowQualityThreshold
		{
			get
			{
				return lowQualityThreshold;
			}
		}

		bool IReadyEffectActivationComponent.effectActive
		{
			get;
			set;
		}

		DispatchOnChange<bool> IReadyEffectActivationComponent.activateReadyEffect
		{
			get
			{
				return _activateReadyEffect;
			}
		}

		Shader ICloakMaterialsComponent.skinnedShader
		{
			get
			{
				return skinnedShader;
			}
		}

		Shader ICloakMaterialsComponent.nonSkinnedShader
		{
			get
			{
				return nonSkinnedShader;
			}
		}

		public CloakModuleMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_activateReadyEffect = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}
	}
}
