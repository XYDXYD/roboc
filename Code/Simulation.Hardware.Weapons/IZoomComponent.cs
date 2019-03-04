namespace Simulation.Hardware.Weapons
{
	internal interface IZoomComponent
	{
		float zoomedFov
		{
			get;
		}

		bool canZoom
		{
			get;
		}

		bool isZoomed
		{
			get;
			set;
		}
	}
}
