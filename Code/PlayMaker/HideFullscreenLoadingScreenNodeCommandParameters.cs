using System;

namespace PlayMaker
{
	public class HideFullscreenLoadingScreenNodeCommandParameters : IPlaymakerCommandInputParameters
	{
		public T GetInputParameters<T>()
		{
			throw new Exception("Error: command requires no input parameters");
		}
	}
}
