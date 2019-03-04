using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class AerofoilImplementor : MonoBehaviour, IAerofoilComponent, IMachineStunComponent, IMaxSpeedComponent, ISpeedModifierComponent, IMaxCarryMassComponent, IValidMovementComponent
	{
		public float BarrelSpeed;

		public float BarrelSpeedHeavy;

		public float BankSpeed;

		public float BankSpeedHeavy;

		public float ElevationSpeed;

		public float ElevationSpeedHeavy;

		public float RudderSpeed;

		public float RudderSpeedHeavy;

		public float Thrust;

		public float ThrustHeavy;

		public float VTOLVelocity;

		public float VTOLVelocityHeavy;

		public Transform ForceT;

		private bool _isValid;

		public Transform forceT => ForceT;

		public float elevationSpeed => ElevationSpeed;

		public float elevationSpeedHeavy => ElevationSpeedHeavy;

		public float barrelSpeed => BarrelSpeed;

		public float barrelSpeedHeavy => BarrelSpeedHeavy;

		public float rudderSpeed => RudderSpeed;

		public float rudderSpeedHeavy => RudderSpeedHeavy;

		public float thrust => Thrust;

		public float thrustHeavy => ThrustHeavy;

		public float bankSpeed => BankSpeed;

		public float bankSpeedHeavy => BankSpeedHeavy;

		public float vtolVelocity => VTOLVelocity;

		public float vtolVelocityHeavy => VTOLVelocityHeavy;

		public bool isFlap
		{
			get;
			set;
		}

		public Dispatcher<IMachineStunComponent, int> machineStunned
		{
			get;
			private set;
		}

		public Dispatcher<IMachineStunComponent, int> remoteMachineStunned
		{
			get;
			private set;
		}

		public float stunTimer
		{
			get;
			set;
		}

		public bool stunned
		{
			get;
			set;
		}

		public int stunningEmpLocator
		{
			get;
			set;
		}

		public float maxSpeed
		{
			get;
			set;
		}

		public Vector3 positiveAxisMaxSpeed
		{
			get;
			set;
		}

		public Vector3 negativeAxisMaxSpeed
		{
			get;
			set;
		}

		public float speedModifier => 1f;

		public bool isValid => _isValid;

		public bool affectsMaxSpeed => true;

		public float maxCarryMass
		{
			get;
			set;
		}

		public float minItemsModifier
		{
			get;
			set;
		}

		public AerofoilImplementor()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			positiveAxisMaxSpeed = Vector3.get_one();
			negativeAxisMaxSpeed = Vector3.get_one();
			minItemsModifier = 1f;
		}

		private void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			_isValid = !(Mathf.Abs(Vector3.Dot(forceT.get_up(), Vector3.get_forward())) > 0.95f);
		}
	}
}
