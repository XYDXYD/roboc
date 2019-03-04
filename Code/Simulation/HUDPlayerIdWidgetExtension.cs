using UnityEngine;

namespace Simulation
{
	internal abstract class HUDPlayerIdWidgetExtension : MonoBehaviour
	{
		protected HUDPlayerIdWidgetExtension()
			: this()
		{
		}

		public abstract void UpdateAlpha(float alpha);
	}
}
