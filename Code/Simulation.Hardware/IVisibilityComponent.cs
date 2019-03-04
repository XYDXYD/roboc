namespace Simulation.Hardware
{
	internal interface IVisibilityComponent
	{
		bool offScreen
		{
			get;
		}

		bool inRange
		{
			get;
		}
	}
}
