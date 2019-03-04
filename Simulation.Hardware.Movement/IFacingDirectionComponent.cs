namespace Simulation.Hardware.Movement
{
	internal interface IFacingDirectionComponent
	{
		CubeFace legacyDirection
		{
			get;
		}

		CubeFace actualDirection
		{
			get;
		}

		CubeFace pitchDirection
		{
			get;
		}
	}
}
