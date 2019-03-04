using Simulation.Hardware.Movement;
using UnityEngine;

internal class WheelReader : MonoBehaviour
{
	public bool grounded;

	public float motorTorque;

	public float brakeTorque;

	public float rpm;

	public float sprungMass;

	public float steerAngle;

	public float forwardStiffness;

	public float sidewaysStiffness;

	public WheelFriction friction;

	private WheelCollider _wheel;

	public WheelReader()
		: this()
	{
	}

	private void OnEnable()
	{
		_wheel = this.GetComponent<WheelCollider>();
	}

	private void Update()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		if (_wheel != null)
		{
			grounded = _wheel.get_isGrounded();
			motorTorque = _wheel.get_motorTorque();
			brakeTorque = _wheel.get_brakeTorque();
			rpm = _wheel.get_rpm();
			sprungMass = _wheel.get_sprungMass();
			steerAngle = _wheel.get_steerAngle();
			WheelFrictionCurve sidewaysFriction = _wheel.get_sidewaysFriction();
			forwardStiffness = sidewaysFriction.get_stiffness();
			WheelFriction wheelFriction = friction;
			WheelFrictionCurve forwardFriction = _wheel.get_forwardFriction();
			wheelFriction.F_AsymptoteSlip = forwardFriction.get_asymptoteSlip();
			WheelFriction wheelFriction2 = friction;
			WheelFrictionCurve forwardFriction2 = _wheel.get_forwardFriction();
			wheelFriction2.F_AsymptoteValue = forwardFriction2.get_asymptoteValue();
			WheelFriction wheelFriction3 = friction;
			WheelFrictionCurve forwardFriction3 = _wheel.get_forwardFriction();
			wheelFriction3.F_ExtremumSlip = forwardFriction3.get_extremumSlip();
			WheelFriction wheelFriction4 = friction;
			WheelFrictionCurve forwardFriction4 = _wheel.get_forwardFriction();
			wheelFriction4.F_ExtremumValue = forwardFriction4.get_extremumValue();
			WheelFrictionCurve sidewaysFriction2 = _wheel.get_sidewaysFriction();
			sidewaysStiffness = sidewaysFriction2.get_stiffness();
			WheelFriction wheelFriction5 = friction;
			WheelFrictionCurve sidewaysFriction3 = _wheel.get_sidewaysFriction();
			wheelFriction5.S_AsymptoteSlip = sidewaysFriction3.get_asymptoteSlip();
			WheelFriction wheelFriction6 = friction;
			WheelFrictionCurve sidewaysFriction4 = _wheel.get_sidewaysFriction();
			wheelFriction6.S_AsymptoteValue = sidewaysFriction4.get_asymptoteValue();
			WheelFriction wheelFriction7 = friction;
			WheelFrictionCurve sidewaysFriction5 = _wheel.get_sidewaysFriction();
			wheelFriction7.S_ExtremumSlip = sidewaysFriction5.get_extremumSlip();
			WheelFriction wheelFriction8 = friction;
			WheelFrictionCurve sidewaysFriction6 = _wheel.get_sidewaysFriction();
			wheelFriction8.S_ExtremumValue = sidewaysFriction6.get_extremumValue();
		}
	}
}
