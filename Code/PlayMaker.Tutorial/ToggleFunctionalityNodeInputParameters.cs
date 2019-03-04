using HutongGames.PlayMaker;
using System;

namespace PlayMaker.Tutorial
{
	public class ToggleFunctionalityNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private FsmEnum _enum;

		private bool _stateToToggleTo;

		public ToggleFunctionalityNodeInputParameters(FsmEnum typeToToggle, bool stateToToggleTo)
		{
			_enum = typeToToggle;
			_stateToToggleTo = stateToToggleTo;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(ToggleFunctionalityNodeType))
			{
				return (T)(object)_enum.get_Value();
			}
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)_stateToToggleTo;
			}
			throw new Exception("Error: there are no input parameters expected for this node.");
		}
	}
}
