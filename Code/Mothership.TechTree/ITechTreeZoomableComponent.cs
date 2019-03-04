using UnityEngine;

namespace Mothership.TechTree
{
	internal interface ITechTreeZoomableComponent
	{
		Transform TreeRoot
		{
			get;
		}

		float MinZoom
		{
			get;
		}

		float MaxZoom
		{
			get;
		}

		float DefaultZoom
		{
			get;
		}

		float ZoomSpeed
		{
			get;
		}
	}
}
