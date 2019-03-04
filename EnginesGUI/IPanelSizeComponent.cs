using Svelto.ECS;
using UnityEngine;

namespace EnginesGUI
{
	internal interface IPanelSizeComponent
	{
		DispatchOnSet<Vector2> PanelSizeChanged
		{
			get;
		}

		int PanelID
		{
			get;
		}

		void Refresh();
	}
}
