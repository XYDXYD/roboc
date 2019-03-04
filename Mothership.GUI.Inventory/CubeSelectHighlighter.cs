using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Mothership.GUI.Inventory
{
	internal sealed class CubeSelectHighlighter
	{
		public Action<CubeTypeID, bool> OnCubeHighlightChanged;

		private HashSet<CubeTypeID> _highlightedCubes = new HashSet<CubeTypeID>();

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		public bool IsHighlighted(CubeTypeID cubeType)
		{
			return _highlightedCubes.Contains(cubeType);
		}

		public void ToggleCubeHighlighting(CubeTypeID cubeTypeID, bool toggleHighlight)
		{
			if (toggleHighlight)
			{
				_highlightedCubes.Add(cubeTypeID);
			}
			else
			{
				_highlightedCubes.Remove(cubeTypeID);
			}
			if (OnCubeHighlightChanged != null)
			{
				OnCubeHighlightChanged(cubeTypeID, toggleHighlight);
			}
		}
	}
}
