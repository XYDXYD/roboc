using HutongGames.PlayMaker;
using System;

namespace PlayMaker.Tutorial
{
	public class ToggleCategoryHighlightNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private bool _highlightSetting;

		private CubeCategory _category;

		public ToggleCategoryHighlightNodeInputParameters(bool highlightSetting, FsmEnum category)
		{
			_highlightSetting = highlightSetting;
			_category = (CubeCategory)(object)category.get_Value();
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)_highlightSetting;
			}
			if (typeof(T) == typeof(CubeCategory))
			{
				return (T)(object)_category;
			}
			throw new Exception("Error: there are no input parameters expected for this node.");
		}
	}
}
