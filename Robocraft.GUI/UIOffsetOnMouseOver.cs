using UnityEngine;

namespace Robocraft.GUI
{
	internal class UIOffsetOnMouseOver : MonoBehaviour
	{
		public UIWidget widgetToOffset;

		public int leftOffset;

		public int rightOffset;

		public int topOffset;

		public int bottomOffset;

		private bool _hovered;

		public UIOffsetOnMouseOver()
			: this()
		{
		}

		private void OnHover(bool isOver)
		{
			if (isOver != _hovered)
			{
				int num = isOver ? 1 : (-1);
				AnchorPoint leftAnchor = widgetToOffset.leftAnchor;
				leftAnchor.absolute += num * leftOffset;
				AnchorPoint rightAnchor = widgetToOffset.rightAnchor;
				rightAnchor.absolute += num * rightOffset;
				AnchorPoint topAnchor = widgetToOffset.topAnchor;
				topAnchor.absolute += num * topOffset;
				AnchorPoint bottomAnchor = widgetToOffset.bottomAnchor;
				bottomAnchor.absolute += num * bottomOffset;
				widgetToOffset.UpdateAnchors();
				_hovered = isOver;
			}
		}

		private void OnDisable()
		{
			if (_hovered)
			{
				OnHover(isOver: false);
			}
		}
	}
}
