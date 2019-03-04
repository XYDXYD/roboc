using UnityEngine;

[RequireComponent(typeof(CharacterMotorEx))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Character/Character Controller")]
internal sealed class CharacterControllerEx : MonoBehaviour
{
	public float VelocityDecay = 1f;

	private Rigidbody RB;

	private Vector3 _initialPosition;

	public CharacterControllerEx()
		: this()
	{
	}

	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		RB = this.GetComponent<Rigidbody>();
		_initialPosition = RB.get_position();
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		RB.set_position(_initialPosition);
	}

	public void Move(Vector3 moveDirection, Quaternion rotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		RB.MoveRotation(rotation);
		RB.MovePosition(this.get_transform().get_position() + moveDirection * Time.get_deltaTime());
	}
}
