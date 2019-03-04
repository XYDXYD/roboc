using System;

namespace PlayMaker.Tutorial
{
	public class RebuildTutorialRobotNodeInputParameters : IPlaymakerCommandInputParameters
	{
		public T GetInputParameters<T>()
		{
			throw new Exception("Error: there are no input parameters expected for this node.");
		}
	}
}
