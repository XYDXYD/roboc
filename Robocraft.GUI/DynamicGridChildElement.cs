using UnityEngine;
using Utility;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIWidget))]
	public class DynamicGridChildElement : MonoBehaviour, IDynamicGridResizingChildElement
	{
		public DynamicGridChildElement()
			: this()
		{
		}

		public void ResizeAccordingToGridRequirements(float expectedWidth, float expectedHeight)
		{
			Console.Log("Resizing widget to: " + expectedWidth + " , " + expectedHeight);
			UIWidget component = this.GetComponent<UIWidget>();
			component.set_height((int)expectedHeight);
			component.set_width((int)expectedWidth);
		}
	}
}
