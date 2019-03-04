using System;
using UnityEngine;

[RequireComponent(typeof(FPSInputControllerEx))]
[RequireComponent(typeof(MouseLookEx))]
[AddComponentMenu("Character/Character Motor")]
internal sealed class CharacterMotorEx : MonoBehaviour
{
	public float MovementSpeed = 1f;

	[NonSerialized]
	public Vector3 LookEulerian = Vector3.get_zero();

	[NonSerialized]
	public Vector3 InputMoveDirection = Vector3.get_zero();

	[NonSerialized]
	public bool InputJump;

	[NonSerialized]
	public bool InputCrouch;

	private CharacterControllerEx _characterController;

	private Vector3 _movementDirection;

	private Transform T;

	public CharacterMotorEx()
		: this()
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)


	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		T = this.get_transform();
		Quaternion rotation = T.get_rotation();
		LookEulerian = rotation.get_eulerAngles();
	}

	private void Start()
	{
		_characterController = this.GetComponent<CharacterControllerEx>();
	}

	public void SetPreviousPositionOrientation(Vector3 pos, Vector3 rot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		LookEulerian = rot;
		T.set_position(pos);
		T.set_rotation(Quaternion.Euler(0f, LookEulerian.y, 0f));
		MouseLookEx component = this.GetComponent<MouseLookEx>();
		component.CameraLook.set_localRotation(Quaternion.Euler(rot.x, 0f, 0f));
	}

	private void FixedUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		_movementDirection = Quaternion.Euler(LookEulerian) * InputMoveDirection;
		if (InputJump)
		{
			ref Vector3 movementDirection = ref _movementDirection;
			movementDirection.y += 1f;
		}
		if (InputCrouch)
		{
			ref Vector3 movementDirection2 = ref _movementDirection;
			movementDirection2.y -= 1f;
		}
		_characterController.Move(_movementDirection.get_normalized() * MovementSpeed, Quaternion.Euler(0f, LookEulerian.y, 0f));
	}
}
