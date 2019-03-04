using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal class PropellerComponentImplementor : MonoBehaviour, ISpinningBladesComponent
	{
		public float _spinForce = 180f;

		public float _spinDeceleration = 5f;

		public Transform _spinningTransform;

		public Transform _blurTransform;

		public Renderer _blurRenderer;

		Renderer ISpinningBladesComponent.blurRenderer
		{
			get
			{
				return _blurRenderer;
			}
		}

		public Transform spinningTransform => _spinningTransform;

		public float currentSpinningSpeed
		{
			get;
			set;
		}

		public float normalizedSpinningSpeed
		{
			get;
			set;
		}

		public Transform blurTransform => _blurTransform;

		public float spinForce => _spinForce;

		public float spinDeceleration => _spinDeceleration;

		public PropellerComponentImplementor()
			: this()
		{
		}
	}
}
