using System;

namespace EnginesGUI
{
	[Serializable]
	public class AnchorConfiguration
	{
		public UIWidget targetLeft;

		public float targetOffsetRelativeLeft;

		public int targetOffsetAbsoluteLeft;

		public UIWidget targetRight;

		public float targetOffsetRelativeRight;

		public int targetOffsetAbsoluteRight;

		public UIWidget targetTop;

		public float targetOffsetRelativeTop;

		public int targetOffsetAbsoluteTop;

		public UIWidget targetBottom;

		public float targetOffsetRelativeBottom;

		public int targetOffsetAbsoluteBottom;
	}
}
