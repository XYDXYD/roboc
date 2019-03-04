namespace Simulation.Hardware
{
	internal interface IThreadSafeTransformComponent
	{
		TransformThreadSafe TTS
		{
			get;
			set;
		}
	}
}
