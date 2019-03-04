using UnityEngine;

[RequireComponent(typeof(CharacterInputWrapper))]
[AddComponentMenu("Character/FPS Input Controller")]
internal sealed class FPSInputControllerEx : MonoBehaviour
{
	private CharacterInputWrapper _input;

	private CharacterMotorEx _motor;

	public FPSInputControllerEx()
		: this()
	{
	}

	private void Awake()
	{
		_input = this.GetComponent<CharacterInputWrapper>();
		_motor = this.GetComponent<CharacterMotorEx>();
	}

	private void Update()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		val._002Ector(_input.horizontalAxis, 0f, _input.verticalAxis);
		if (val != Vector3.get_zero())
		{
			float magnitude = val.get_magnitude();
			val /= magnitude;
			magnitude = Mathf.Min(1f, magnitude);
			val *= magnitude;
		}
		_motor.InputMoveDirection = val;
		_motor.InputJump = _input.jump;
		_motor.InputCrouch = _input.crouch;
	}
}
