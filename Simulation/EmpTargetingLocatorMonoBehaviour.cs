using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	public class EmpTargetingLocatorMonoBehaviour : MonoBehaviour, IEmpLocatorCoutdownComponent, IEmpLocatorObjectComponent, IEmpLocatorTransformComponent, IEmpStunActivationComponent, IEmpLocatorOwnerComponent, IEmpLocatorRangeComponent, IEmpStunDurationComponent, IEmpLocatorEffectsComponent
	{
		[SerializeField]
		private GameObject _stunMainEffectAllyPrefab;

		[SerializeField]
		private GameObject _stunMainEffectEnemyPrefab;

		[SerializeField]
		private GameObject _machineStunEffectAllyPrefab;

		[SerializeField]
		private GameObject _machineStunEffectEnemyPrefab;

		[SerializeField]
		private GameObject _machineRecoverEffectAllyPrefab;

		[SerializeField]
		private GameObject _machineRecoverEffectEnemyPrefab;

		[SerializeField]
		private GameObject _crackDecalAllyPrefab;

		[SerializeField]
		private GameObject _crackDecalEnemyPrefab;

		[SerializeField]
		private GameObject _glowFloorEffectAllyPrefab;

		[SerializeField]
		private GameObject _glowFloorEffectAllyPrefabGood;

		[SerializeField]
		private GameObject _glowFloorEffectAllyPrefabFantastic;

		[SerializeField]
		private GameObject _glowFloorEffectEnemyPrefab;

		[SerializeField]
		private GameObject _glowFloorEffectEnemyPrefabGood;

		[SerializeField]
		private GameObject _glowFloorEffectEnemyPrefabFantastic;

		private int _ownerId;

		private int _ownerMachineId;

		private bool _isOnMyTeam;

		private float _countdown;

		private float _stunDuration;

		private float _range;

		private float _countdownTimer;

		private Dispatcher<IEmpStunActivationComponent, int> _activateEmpStun;

		private Animator _animator;

		private float _currentAppliedSpeedFactor = -1f;

		private Dispatcher<IEmpLocatorEffectsComponent, GlowFloorEffectData> _playGlowFloorEffect;

		float IEmpLocatorCoutdownComponent.countdown
		{
			get
			{
				return _countdown;
			}
		}

		float IEmpLocatorCoutdownComponent.countdownTimer
		{
			get
			{
				return _countdownTimer;
			}
			set
			{
				_countdownTimer = value;
			}
		}

		GameObject IEmpLocatorObjectComponent.empLocatorObject
		{
			get
			{
				return this.get_gameObject();
			}
		}

		Transform IEmpLocatorTransformComponent.empLocatorTransform
		{
			get
			{
				return this.get_transform();
			}
		}

		Dispatcher<IEmpStunActivationComponent, int> IEmpStunActivationComponent.activateEmpStun
		{
			get
			{
				return _activateEmpStun;
			}
		}

		int IEmpLocatorOwnerComponent.ownerId
		{
			get
			{
				return _ownerId;
			}
		}

		int IEmpLocatorOwnerComponent.ownerMachineId
		{
			get
			{
				return _ownerMachineId;
			}
		}

		bool IEmpLocatorOwnerComponent.isOnMyTeam
		{
			get
			{
				return _isOnMyTeam;
			}
		}

		float IEmpLocatorRangeComponent.range
		{
			get
			{
				return _range;
			}
		}

		float IEmpStunDurationComponent.stunDuration
		{
			get
			{
				return _stunDuration;
			}
		}

		public GameObject stunMainEffectAllyPrefab => _stunMainEffectAllyPrefab;

		public GameObject stunMainEffectEnemyPrefab => _stunMainEffectEnemyPrefab;

		public GameObject machineStunEffectAllyPrefab => _machineStunEffectAllyPrefab;

		public GameObject machineStunEffectEnemyPrefab => _machineStunEffectEnemyPrefab;

		public GameObject machineRecoverEffectAllyPrefab => _machineRecoverEffectAllyPrefab;

		public GameObject machineRecoverEffectEnemyPrefab => _machineRecoverEffectEnemyPrefab;

		public GameObject crackDecalAllyPrefab => _crackDecalAllyPrefab;

		public GameObject crackDecalEnemyPrefab => _crackDecalEnemyPrefab;

		public GameObject glowFloorEffectAllyPrefab => QualitySelector(_glowFloorEffectAllyPrefab, _glowFloorEffectAllyPrefabGood, _glowFloorEffectAllyPrefabFantastic);

		public GameObject glowFloorEffectEnemyPrefab => QualitySelector(_glowFloorEffectEnemyPrefab, _glowFloorEffectEnemyPrefabGood, _glowFloorEffectEnemyPrefabFantastic);

		public Dispatcher<IEmpLocatorEffectsComponent, GlowFloorEffectData> playGlowFloorEffect => _playGlowFloorEffect;

		public EmpTargetingLocatorMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_activateEmpStun = new Dispatcher<IEmpStunActivationComponent, int>(this);
			_playGlowFloorEffect = new Dispatcher<IEmpLocatorEffectsComponent, GlowFloorEffectData>(this);
			_animator = this.GetComponentInChildren<Animator>();
		}

		public void SetOwner(int ownerId, int ownerMachineId, bool isOnMyTeam)
		{
			_ownerId = ownerId;
			_ownerMachineId = ownerMachineId;
			_isOnMyTeam = isOnMyTeam;
		}

		public void SetTimeVariables(float countdown, float stunDuration)
		{
			_countdown = countdown;
			_countdownTimer = countdown;
			_stunDuration = stunDuration;
			float length = _animator.get_runtimeAnimatorController().get_animationClips()[0].get_length();
			float num = length / _countdown;
			if (num != _currentAppliedSpeedFactor)
			{
				Animator animator = _animator;
				animator.set_speed(animator.get_speed() * num);
				_currentAppliedSpeedFactor = num;
			}
		}

		public void SetRange(float range)
		{
			_range = range;
		}

		private GameObject QualitySelector(GameObject normal, GameObject good, GameObject Beautiful)
		{
			switch (QualitySettings.get_names()[QualitySettings.GetQualityLevel()])
			{
			case "Fantastic":
			case "Beautiful":
				return Beautiful;
			case "Good":
				return good;
			default:
				return normal;
			}
		}
	}
}
