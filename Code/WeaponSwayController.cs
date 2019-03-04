using System;
using UnityEngine;

internal class WeaponSwayController : MonoBehaviour
{
	public GameObject weaponsParentObject;

	public GameObject paintGun;

	public GameObject buildGun;

	public float idleSwaySpeed = 1f;

	public float idleSwayXScale = 0.03f;

	public float idleSwayYScale = 0.01f;

	public float movingSwaySpeed = 3f;

	public float movingSwayXScale = 0.01f;

	public float movingSwayYScale = 0.05f;

	public float movingUpAngle = -30f;

	public float movingDownAngle = 30f;

	public float maxSwayTransitionTime = 0.5f;

	public float maxPitchTransitionTime = 0.2f;

	private Vector3 _pivot;

	private Vector3 _pivotOffset;

	private float _phase;

	private bool _invert;

	private float _2PI = (float)Math.PI * 2f;

	private float _speed = 1f;

	private float _xScale = 1f;

	private float _yScale = 1f;

	private float _angle;

	private bool _moving;

	private bool _movingUp;

	private bool _movingDown;

	private bool _shooting;

	private bool _lerpingSway;

	private bool _lerpingPitch;

	private float _lerpingSwayTime;

	private float _lerpingPitchTime;

	private Quaternion localRotation = default(Quaternion);

	private Quaternion originalRotation;

	private CharacterMotorEx _characterMotor;

	public WeaponSwayController()
		: this()
	{
	}//IL_009d: Unknown result type (might be due to invalid IL or missing references)
	//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
	//IL_00a4: Unknown result type (might be due to invalid IL or missing references)


	private void Start()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_pivot = weaponsParentObject.get_transform().get_localPosition();
		originalRotation = paintGun.get_transform().get_localRotation();
		_characterMotor = this.GetComponent<CharacterMotorEx>();
		_speed = idleSwaySpeed;
		_xScale = idleSwayXScale;
		_yScale = idleSwayYScale;
	}

	private void Update()
	{
		if (!_shooting)
		{
			HandleMotorInput();
			SetLateralMovementVariables();
			UpdateSway();
			PitchWeaponOnVerticalMovement();
		}
	}

	private void UpdateSway()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		_phase += _speed * Time.get_deltaTime();
		if (_phase > _2PI)
		{
			_invert = !_invert;
			_phase -= _2PI;
			_phase += _speed * Time.get_deltaTime();
		}
		if (_phase < 0f)
		{
			_phase += _2PI;
		}
		_pivotOffset = Vector3.get_left() * _xScale * (float)(_invert ? 1 : (-1));
		Vector3 localPosition = _pivot + _pivotOffset;
		localPosition.x += Mathf.Sin(_phase - _2PI * 0.25f) * _xScale * (float)((!_invert) ? 1 : (-1));
		localPosition.y += Mathf.Cos(_phase - _2PI * 0.25f) * _yScale;
		weaponsParentObject.get_transform().set_localPosition(localPosition);
	}

	private void PitchWeaponOnVerticalMovement()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (_lerpingPitch)
		{
			_lerpingPitchTime += Time.get_deltaTime();
			float num = _lerpingPitchTime / maxPitchTransitionTime;
			if (num > 1f)
			{
				_lerpingPitch = false;
			}
			float num2 = 0f;
			if (_movingDown)
			{
				num2 += movingDownAngle;
			}
			if (_movingUp)
			{
				num2 += movingUpAngle;
			}
			_angle = Mathf.Lerp(_angle, num2, num);
		}
		Vector3 zero = Vector3.get_zero();
		zero.z = _angle;
		localRotation.set_eulerAngles(zero);
		paintGun.get_transform().set_localRotation(originalRotation * localRotation);
		buildGun.get_transform().set_localRotation(originalRotation * localRotation);
	}

	private void SetLateralMovementVariables()
	{
		if (_lerpingSway)
		{
			_lerpingSwayTime += Time.get_deltaTime();
			float num = _lerpingSwayTime / maxSwayTransitionTime;
			if (num > 1f)
			{
				_lerpingSway = false;
			}
			if (_moving)
			{
				_speed = Mathf.Lerp(_speed, movingSwaySpeed, num);
				_xScale = Mathf.Lerp(_xScale, movingSwayXScale, num);
				_yScale = Mathf.Lerp(_yScale, movingSwayYScale, num);
			}
			else
			{
				_speed = Mathf.Lerp(_speed, idleSwaySpeed, num);
				_xScale = Mathf.Lerp(_xScale, idleSwayXScale, num);
				_yScale = Mathf.Lerp(_yScale, idleSwayYScale, num);
			}
		}
	}

	private void HandleMotorInput()
	{
		if (_moving != _characterMotor.InputMoveDirection.get_sqrMagnitude() > 0.1f)
		{
			_moving = (_characterMotor.InputMoveDirection.get_sqrMagnitude() > 0.1f);
			_lerpingSway = true;
			_lerpingSwayTime = 0f;
		}
		if (_movingDown != _characterMotor.InputCrouch || _movingUp != _characterMotor.InputJump)
		{
			_movingDown = _characterMotor.InputCrouch;
			_movingUp = _characterMotor.InputJump;
			_lerpingPitch = true;
			_lerpingPitchTime = 0f;
		}
	}
}
