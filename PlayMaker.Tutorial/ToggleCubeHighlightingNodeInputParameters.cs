using System;

namespace PlayMaker.Tutorial
{
	public class ToggleCubeHighlightingNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private bool _highlightSetting;

		private CubeTypeID _cubeTypeId;

		public ToggleCubeHighlightingNodeInputParameters(bool highlightSetting, string cubeTypeString)
		{
			_highlightSetting = highlightSetting;
			_cubeTypeId = Convert.ToUInt32(cubeTypeString, 16);
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)_highlightSetting;
			}
			if (typeof(T) == typeof(CubeTypeID))
			{
				return (T)(object)_cubeTypeId;
			}
			throw new Exception("Error: there are no input parameters expected for this node.");
		}
	}
}
