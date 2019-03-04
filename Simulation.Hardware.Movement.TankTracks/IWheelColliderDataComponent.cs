using Svelto.DataStructures;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IWheelColliderDataComponent
	{
		FasterList<WheelColliderData> wheelColliders
		{
			get;
		}
	}
}
