namespace Simulation
{
	internal interface ITeleportModuleSettingsComponent
	{
		float teleportTime
		{
			get;
			set;
		}

		float cameraTime
		{
			get;
			set;
		}

		float cameraDelay
		{
			get;
			set;
		}
	}
}
