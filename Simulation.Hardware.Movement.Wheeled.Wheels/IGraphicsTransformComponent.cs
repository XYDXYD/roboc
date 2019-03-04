using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal interface IGraphicsTransformComponent
	{
		Transform suspensionTransform
		{
			get;
		}

		Transform wheelToRotateTransform
		{
			get;
		}

		Transform steeringNodeTransform
		{
			get;
		}
	}
}
