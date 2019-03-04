using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal interface IGraphicsTransformComponent
	{
		Transform suspensionTransform
		{
			get;
		}

		Transform hingeTransform
		{
			get;
		}

		Transform steeringNodeTransform
		{
			get;
		}
	}
}
