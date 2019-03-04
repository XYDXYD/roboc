using UnityEngine;

namespace Simulation.Hardware.Cosmetic.Eye
{
	internal class EyeImplementor : MonoBehaviour, IEyeComponent, IVisibilityComponent, IVisibilityTracker, IImplementor
	{
		public Transform[] Lids;

		public Vector3[] Axis;

		public float[] RotateAmounts;

		public Transform EyeBall;

		private bool _offScreen;

		private bool _inRange;

		public bool isOffScreen
		{
			get
			{
				return _offScreen;
			}
			set
			{
				_offScreen = value;
			}
		}

		public bool isInRange
		{
			get
			{
				return _inRange;
			}
			set
			{
				_inRange = value;
			}
		}

		public bool offScreen => _offScreen;

		public bool inRange => _inRange;

		public Transform[] lids => Lids;

		public Vector3[] axis => Axis;

		public float[] rotateAmounts => RotateAmounts;

		public Transform eyeBall => EyeBall;

		public EyeImplementor()
			: this()
		{
		}
	}
}
