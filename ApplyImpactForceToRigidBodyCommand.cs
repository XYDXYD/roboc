using Svelto.Command;
using UnityEngine;

internal sealed class ApplyImpactForceToRigidBodyCommand : ICommand
{
	private Rigidbody _rigidBody;

	private Vector3 _direction;

	private float _scale;

	private Vector3 _hitPosition;

	public ApplyImpactForceToRigidBodyCommand(Rigidbody rigidBody, Vector3 direction, float scale, Vector3 hitPosition)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_rigidBody = rigidBody;
		_direction = direction;
		_scale = scale;
		_hitPosition = hitPosition;
	}

	public void Execute()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (_rigidBody != null)
		{
			_rigidBody.AddForceAtPosition(_direction.get_normalized() * _scale, _hitPosition, 1);
		}
	}
}
