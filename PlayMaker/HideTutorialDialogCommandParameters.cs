using HutongGames.PlayMaker;
using System;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("create a gameobject FSM from within a FSM and also handle injection correctly.")]
	public class HideTutorialDialogCommandParameters : IPlaymakerCommandInputParameters
	{
		public T GetInputParameters<T>()
		{
			throw new Exception("Error: command requires no input parameters");
		}
	}
}
