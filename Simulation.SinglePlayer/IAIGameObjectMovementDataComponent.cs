using UnityEngine;

namespace Simulation.SinglePlayer
{
	public interface IAIGameObjectMovementDataComponent
	{
		Vector3 position
		{
			get;
		}

		Vector3 velocity
		{
			get;
		}

		Vector3 forward
		{
			get;
		}

		float horizontalRadius
		{
			get;
		}

		Rigidbody rigidBody
		{
			get;
		}

		GameObject root
		{
			get;
		}

		Vector3[] minmax
		{
			get;
		}

		float maxspeed
		{
			get;
		}

		float maxTurningSpeed
		{
			get;
		}

		int playeId
		{
			get;
		}

		string playerName
		{
			get;
		}

		int teamId
		{
			get;
		}

		bool isVisible
		{
			get;
			set;
		}

		Transform cameraPivotTransform
		{
			get;
		}
	}
}
