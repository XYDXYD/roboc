using UnityEngine;

namespace Simulation.GUI
{
	internal class HitHighlight : MonoBehaviour
	{
		[SerializeField]
		private UISprite _highlightSprite;

		[SerializeField]
		private Animation _highlightAnimation;

		public UISprite highlightSprite => _highlightSprite;

		public Animation highlightAnimation => _highlightAnimation;

		public HitHighlight()
			: this()
		{
		}
	}
}
