using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	internal sealed class ShieldEntity : MonoBehaviour, IDiscShieldOwnerComponent, IDiscShieldJustSpawnedComponent, IDiscShieldObjectComponent, IDiscShieldSettingsComponent, IDiscShieldEffectsComponent, IDiscShieldAudioComponent, IDiscShieldActivationTimeComponent, IDiscShieldClosingTimeComponent
	{
		[SerializeField]
		private Renderer discShieldRenderer;

		[SerializeField]
		private Renderer ringEffectRenderer;

		[SerializeField]
		private string shieldOnSoundName = "DiscShield_ON";

		[SerializeField]
		private string shieldOffSoundName = "DiscShield_OFF";

		[SerializeField]
		private string shieldLoopSoundName = "DiscShield_Loop";

		[SerializeField]
		private string shieldGlowLoopSoundName = "DiscShield_Glow_Loop";

		[SerializeField]
		private string shieldGlowLoopSoundParameterName = "DiscShield_GlowNumber";

		[SerializeField]
		private float openTime = 0.5f;

		[SerializeField]
		private float closeTime = 0.5f;

		[SerializeField]
		private float ringOpenTime = 0.5f;

		[SerializeField]
		private float ringCloseTime = 0.5f;

		[SerializeField]
		private float nearToCloseEffectStartTime = 6f;

		[SerializeField]
		private float groundOffset;

		private int _ownerId;

		private bool _isMine;

		private bool _isMyTeam;

		private bool _justSpawned;

		private float _activationTime;

		private float _shieldLifeTime;

		private Dispatcher<IDiscShieldEffectsComponent, int> _startOpenEffect;

		private Dispatcher<IDiscShieldEffectsComponent, int> _startNearToCloseEffect;

		private Dispatcher<IDiscShieldEffectsComponent, int> _startCloseEffect;

		private Dispatcher<IDiscShieldAudioComponent, int> _playOnSound;

		private Dispatcher<IDiscShieldAudioComponent, int> _playOffSound;

		private Dispatcher<IDiscShieldAudioComponent, int> _playLoopSound;

		private Dispatcher<IDiscShieldAudioComponent, int> _playGlowLoopSound;

		private Dispatcher<IDiscShieldAudioComponent, int> _stopLoopSound;

		private Dispatcher<IDiscShieldAudioComponent, int> _stopGlowLoopSound;

		private Dispatcher<IDiscShieldAudioComponent, SoundParameterData> _setGlowLoopSoundParameter;

		int IDiscShieldOwnerComponent.discShieldOwner
		{
			get
			{
				return _ownerId;
			}
		}

		bool IDiscShieldOwnerComponent.isMine
		{
			get
			{
				return _isMine;
			}
		}

		bool IDiscShieldOwnerComponent.isOnMyTeam
		{
			get
			{
				return _isMyTeam;
			}
		}

		bool IDiscShieldJustSpawnedComponent.discShieldJustSpawned
		{
			get
			{
				return _justSpawned;
			}
			set
			{
				_justSpawned = value;
			}
		}

		ShieldEntity IDiscShieldObjectComponent.discShieldObject
		{
			get
			{
				return this;
			}
		}

		bool IDiscShieldObjectComponent.shieldActive
		{
			get
			{
				return this.get_gameObject().get_activeSelf();
			}
			set
			{
				this.get_gameObject().SetActive(value);
			}
		}

		float IDiscShieldSettingsComponent.discShieldLifeTime
		{
			get
			{
				return _shieldLifeTime;
			}
			set
			{
				_shieldLifeTime = value;
			}
		}

		float IDiscShieldEffectsComponent.openTime
		{
			get
			{
				return openTime;
			}
		}

		float IDiscShieldEffectsComponent.closeTime
		{
			get
			{
				return closeTime;
			}
		}

		Renderer IDiscShieldEffectsComponent.discShieldRenderer
		{
			get
			{
				return discShieldRenderer;
			}
		}

		Renderer IDiscShieldEffectsComponent.ringEffectRenderer
		{
			get
			{
				return ringEffectRenderer;
			}
		}

		float IDiscShieldEffectsComponent.ringOpenTime
		{
			get
			{
				return ringOpenTime;
			}
		}

		float IDiscShieldEffectsComponent.ringCloseTime
		{
			get
			{
				return ringCloseTime;
			}
		}

		float IDiscShieldEffectsComponent.nearToCloseEffectStartTime
		{
			get
			{
				return nearToCloseEffectStartTime;
			}
		}

		Dispatcher<IDiscShieldEffectsComponent, int> IDiscShieldEffectsComponent.startOpenEffect
		{
			get
			{
				return _startOpenEffect;
			}
		}

		Dispatcher<IDiscShieldEffectsComponent, int> IDiscShieldEffectsComponent.startNearToCloseEffect
		{
			get
			{
				return _startNearToCloseEffect;
			}
		}

		Dispatcher<IDiscShieldEffectsComponent, int> IDiscShieldEffectsComponent.startCloseEffect
		{
			get
			{
				return _startCloseEffect;
			}
		}

		float IDiscShieldEffectsComponent.openingTimer
		{
			get;
			set;
		}

		float IDiscShieldEffectsComponent.closingTimer
		{
			get;
			set;
		}

		float IDiscShieldEffectsComponent.ringOpeningTimer
		{
			get;
			set;
		}

		float IDiscShieldEffectsComponent.ringClosingTimer
		{
			get;
			set;
		}

		float IDiscShieldEffectsComponent.nearToCloseEffectsTimer
		{
			get;
			set;
		}

		string IDiscShieldAudioComponent.shieldOnSoundName
		{
			get
			{
				return shieldOnSoundName;
			}
		}

		string IDiscShieldAudioComponent.shieldOffSoundName
		{
			get
			{
				return shieldOffSoundName;
			}
		}

		string IDiscShieldAudioComponent.shieldLoopSoundName
		{
			get
			{
				return shieldLoopSoundName;
			}
		}

		string IDiscShieldAudioComponent.shieldGlowLoopSoundName
		{
			get
			{
				return shieldGlowLoopSoundName;
			}
		}

		string IDiscShieldAudioComponent.shieldGlowLoopSoundParameterName
		{
			get
			{
				return shieldGlowLoopSoundParameterName;
			}
		}

		Dispatcher<IDiscShieldAudioComponent, int> IDiscShieldAudioComponent.playOnSound
		{
			get
			{
				return _playOnSound;
			}
		}

		Dispatcher<IDiscShieldAudioComponent, int> IDiscShieldAudioComponent.playOffSound
		{
			get
			{
				return _playOffSound;
			}
		}

		Dispatcher<IDiscShieldAudioComponent, int> IDiscShieldAudioComponent.playLoopSound
		{
			get
			{
				return _playLoopSound;
			}
		}

		Dispatcher<IDiscShieldAudioComponent, int> IDiscShieldAudioComponent.playGlowLoopSound
		{
			get
			{
				return _playGlowLoopSound;
			}
		}

		Dispatcher<IDiscShieldAudioComponent, int> IDiscShieldAudioComponent.stopLoopSound
		{
			get
			{
				return _stopLoopSound;
			}
		}

		Dispatcher<IDiscShieldAudioComponent, int> IDiscShieldAudioComponent.stopGlowLoopSound
		{
			get
			{
				return _stopGlowLoopSound;
			}
		}

		Dispatcher<IDiscShieldAudioComponent, SoundParameterData> IDiscShieldAudioComponent.setGlowLoopSoundParameter
		{
			get
			{
				return _setGlowLoopSoundParameter;
			}
		}

		float IDiscShieldActivationTimeComponent.activationTime
		{
			get
			{
				return _activationTime;
			}
			set
			{
				_activationTime = value;
			}
		}

		float IDiscShieldClosingTimeComponent.closingTime
		{
			get;
			set;
		}

		public ShieldEntity()
			: this()
		{
		}

		private void Awake()
		{
			_startOpenEffect = new Dispatcher<IDiscShieldEffectsComponent, int>(this);
			_startNearToCloseEffect = new Dispatcher<IDiscShieldEffectsComponent, int>(this);
			_startCloseEffect = new Dispatcher<IDiscShieldEffectsComponent, int>(this);
			_playOnSound = new Dispatcher<IDiscShieldAudioComponent, int>(this);
			_playOffSound = new Dispatcher<IDiscShieldAudioComponent, int>(this);
			_playLoopSound = new Dispatcher<IDiscShieldAudioComponent, int>(this);
			_playGlowLoopSound = new Dispatcher<IDiscShieldAudioComponent, int>(this);
			_stopLoopSound = new Dispatcher<IDiscShieldAudioComponent, int>(this);
			_stopGlowLoopSound = new Dispatcher<IDiscShieldAudioComponent, int>(this);
			_setGlowLoopSoundParameter = new Dispatcher<IDiscShieldAudioComponent, SoundParameterData>(this);
		}

		private void Start()
		{
			discShieldRenderer.get_material().SetFloat("_activate", 0f);
			discShieldRenderer.get_material().SetFloat("_unstable", 0f);
			ringEffectRenderer.get_material().SetFloat("_Range", 1f);
		}

		public void SetOwnership(int ownerId, bool isMine, bool isMyTeam)
		{
			_ownerId = ownerId;
			_isMine = isMine;
			_isMyTeam = isMyTeam;
		}

		public void SetJustSpawned(bool justSpawned)
		{
			_justSpawned = justSpawned;
		}

		public float GetDiscShieldGroundOffset()
		{
			return groundOffset;
		}
	}
}
