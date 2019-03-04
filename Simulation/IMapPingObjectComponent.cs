using Svelto.ES.Legacy;
using UnityEngine;

namespace Simulation
{
	internal interface IMapPingObjectComponent : IComponent
	{
		Color GetMapPingInitialColor();

		Vector3 GetMapPingInitialScale();

		float GetCameraDistance();

		float GetDecresingColorPercentage();

		float GetScalingUpPercentage();

		void SetMapPingScale(Vector3 scale);

		void SetMapPingColor(Color color, Color transparentColor);

		void SetMapPingLabel(string userName);
	}
}
