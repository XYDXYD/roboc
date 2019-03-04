using Svelto.ECS;
using Svelto.ECS.Legacy;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class TeleportModuleComponentImplementator : MonoBehaviour, IReadyEffectActivationComponent, ITeleporterComponent, ITeleportModuleEffectsComponent, ITeleportModuleSettingsComponent, IRemoveEntityComponent
	{
		[SerializeField]
		private Animator readyEffectAnimator;

		[SerializeField]
		private GameObject _teleportStartEffectAlly;

		[SerializeField]
		private GameObject _teleportTrailEffectAlly;

		[SerializeField]
		private GameObject _teleportGlowingCenterEffectAlly;

		[SerializeField]
		private GameObject _teleportEndEffectAlly;

		[SerializeField]
		private GameObject _teleportStartEffectEnemy;

		[SerializeField]
		private GameObject _teleportTrailEffectEnemy;

		[SerializeField]
		private GameObject _teleportGlowingCenterEffectEnemy;

		[SerializeField]
		private GameObject _teleportEndEffectEnemy;

		private DispatchOnChange<bool> _activateReadyEffect;

		private Dispatcher<ITeleporterComponent, int> _teleportStarted;

		private Dispatcher<ITeleporterComponent, int> _teleportEnded;

		private Vector3 _destination;

		private float _distance;

		private float _teleportTimer;

		private bool _teleportActivated;

		private float _teleportTime;

		private float _cameraTime;

		private float _cameraDelay;

		DispatchOnChange<bool> IReadyEffectActivationComponent.activateReadyEffect
		{
			get
			{
				return _activateReadyEffect;
			}
		}

		bool IReadyEffectActivationComponent.effectActive
		{
			get
			{
				return readyEffectAnimator.GetBool("Activate");
			}
			set
			{
				readyEffectAnimator.SetBool("Activate", value);
			}
		}

		public Dispatcher<ITeleporterComponent, int> teleportStarted => _teleportStarted;

		public Dispatcher<ITeleporterComponent, int> teleportEnded => _teleportEnded;

		public Vector3 destination
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _destination;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_destination = value;
			}
		}

		public float distance
		{
			get
			{
				return _distance;
			}
			set
			{
				_distance = value;
			}
		}

		public float teleportTimer
		{
			get
			{
				return _teleportTimer;
			}
			set
			{
				_teleportTimer = value;
			}
		}

		public bool teleportActivated
		{
			get
			{
				return _teleportActivated;
			}
			set
			{
				_teleportActivated = value;
			}
		}

		public GameObject teleportStartEffectAlly => _teleportStartEffectAlly;

		public GameObject teleportTrailEffectAlly => _teleportTrailEffectAlly;

		public GameObject teleportGlowingCenterEffectAlly => _teleportGlowingCenterEffectAlly;

		public GameObject teleportEndEffectAlly => _teleportEndEffectAlly;

		public GameObject teleportStartEffectEnemy => _teleportStartEffectEnemy;

		public GameObject teleportTrailEffectEnemy => _teleportTrailEffectEnemy;

		public GameObject teleportGlowingCenterEffectEnemy => _teleportGlowingCenterEffectEnemy;

		public GameObject teleportEndEffectEnemy => _teleportEndEffectEnemy;

		public float teleportTime
		{
			get
			{
				return _teleportTime;
			}
			set
			{
				_teleportTime = value;
			}
		}

		public float cameraTime
		{
			get
			{
				return _cameraTime;
			}
			set
			{
				_cameraTime = value;
			}
		}

		public float cameraDelay
		{
			get
			{
				return _cameraDelay;
			}
			set
			{
				_cameraDelay = value;
			}
		}

		public Action removeEntity
		{
			get;
			set;
		}

		public TeleportModuleComponentImplementator()
			: this()
		{
		}

		private void Awake()
		{
			_activateReadyEffect = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_teleportStarted = new Dispatcher<ITeleporterComponent, int>(this);
			_teleportEnded = new Dispatcher<ITeleporterComponent, int>(this);
		}
	}
}
