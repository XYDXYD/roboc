using System;
using UnityEngine;

namespace Simulation.SinglePlayer
{
	[Serializable]
	internal class AIGameObjectMovementData : IAIGameObjectMovementDataComponent
	{
		private Rigidbody _rigidBody;

		private float _horizontalRadius;

		private Vector3[] _minMax;

		private GameObject _root;

		private float _maxSpeed;

		private float _maxTurningSpeed;

		public Vector3 position => _rigidBody.get_worldCenterOfMass();

		public Vector3 velocity => _rigidBody.get_velocity();

		public Vector3 forward => _rigidBody.get_transform().get_forward();

		public float horizontalRadius => _horizontalRadius;

		public Rigidbody rigidBody => _rigidBody;

		public GameObject root => _root;

		public Vector3[] minmax => _minMax;

		public float maxspeed => _maxSpeed;

		public float maxTurningSpeed => _maxTurningSpeed;

		public int playeId
		{
			get;
			set;
		}

		public string playerName
		{
			get;
			set;
		}

		public int teamId
		{
			get;
			set;
		}

		public bool isVisible
		{
			get;
			set;
		}

		public TargetInfo targetInfo
		{
			get;
			set;
		}

		public Transform cameraPivotTransform
		{
			get;
			set;
		}

		public void SetHorizontalRadius(float rd)
		{
			_horizontalRadius = rd;
		}

		public void SetRigidBody(Rigidbody rb)
		{
			_rigidBody = rb;
		}

		public void SetRoot(GameObject root)
		{
			_root = root;
		}

		public void SetminMax(Vector3[] minmax_)
		{
			_minMax = minmax_;
		}

		public void SetMaxSpeed(float maxSpeed_)
		{
			_maxSpeed = maxSpeed_;
		}

		public void SetMaxTurningSpeed(float maxTurningSpeed_)
		{
			_maxTurningSpeed = maxTurningSpeed_;
		}
	}
}
