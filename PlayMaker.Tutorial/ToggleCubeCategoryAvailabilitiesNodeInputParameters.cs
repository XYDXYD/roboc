using System;

namespace PlayMaker.Tutorial
{
	public class ToggleCubeCategoryAvailabilitiesNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private bool[] categoryAvailabilities = new bool[Enum.GetNames(typeof(CubeCategory)).Length];

		public ToggleCubeCategoryAvailabilitiesNodeInputParameters(bool chassisAvailable, bool drivingAvailable, bool specialAvailable, bool hardwareAvailable, bool cosmeticAvailable)
		{
			categoryAvailabilities[1] = chassisAvailable;
			categoryAvailabilities[2] = drivingAvailable;
			categoryAvailabilities[3] = specialAvailable;
			categoryAvailabilities[4] = hardwareAvailable;
			categoryAvailabilities[5] = cosmeticAvailable;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(bool[]))
			{
				return (T)(object)categoryAvailabilities;
			}
			throw new Exception("Error: there are no input parameters expected for this node.");
		}
	}
}
