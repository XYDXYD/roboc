using System;

namespace PlayMaker
{
	public class ShowFullscreenLoadingScreenNodeCommandParameters : IPlaymakerCommandInputParameters
	{
		public T GetInputParameters<T>()
		{
			throw new Exception("Error: command requires no input parameters");
		}
	}
}
