using UnityEngine;

namespace Simulation.BattleArena.Equalizer
{
	internal class EqualizerImplementor : MonoBehaviour, IRigidbodyComponent, IMachineMapComponent, IOwnerComponent, IAnimatorComponent, IMaterialComponent, IGodRayComponent, IRootComponent, IAudioComponent, IVisualTeamComponent, IOwnerTeamComponent
	{
		public Animator _animator;

		public Renderer _renderer;

		public string _healthProperty = "_Ramp";

		public string _crackProperty = "_CrackStr";

		public GameObject[] _godRays;

		public string _loopAudioEvent;

		public string _loopAudioParameter;

		private int _healthPropertyId;

		private int _crackPropertyId;

		private Material _material;

		public int crackPropertyId => _crackPropertyId;

		public int healthPropertyId => _healthPropertyId;

		public int machineId
		{
			get;
			set;
		}

		public bool onMyTeam
		{
			get;
			set;
		}

		public IMachineMap machineMap
		{
			get;
			set;
		}

		public Material material => _material;

		public int playerId
		{
			get;
			set;
		}

		public Rigidbody rb
		{
			get;
			set;
		}

		public Vector3 activePosition
		{
			get;
			set;
		}

		public Vector3 inactivePosition
		{
			get;
			set;
		}

		public Animator animator => _animator;

		public GameObject[] godRays => _godRays;

		public GameObject root
		{
			get;
			set;
		}

		public string loopAudioEvent => _loopAudioEvent;

		public string loopAudioParameter => _loopAudioParameter;

		public int ownerTeamId
		{
			get;
			set;
		}

		public VisualTeam visualTeam
		{
			get;
			set;
		}

		public EqualizerImplementor()
			: this()
		{
		}

		private void Awake()
		{
			_healthPropertyId = Shader.PropertyToID(_healthProperty);
			_crackPropertyId = Shader.PropertyToID(_crackProperty);
			_material = _renderer.get_sharedMaterial();
		}
	}
}
