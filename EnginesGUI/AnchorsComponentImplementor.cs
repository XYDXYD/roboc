using UnityEngine;

namespace EnginesGUI
{
	public class AnchorsComponentImplementor : MonoBehaviour, IAnchorsComponent
	{
		private UIWidget _widget;

		public AnchorsComponentImplementor()
			: this()
		{
		}

		public void Awake()
		{
			_widget = this.GetComponent<UIWidget>();
		}

		public void ApplyAnchors(AnchorConfiguration anchors)
		{
			if (anchors.targetLeft != null)
			{
				_widget.leftAnchor.Set(anchors.targetLeft.get_transform(), anchors.targetOffsetRelativeLeft, (float)anchors.targetOffsetAbsoluteLeft);
			}
			if (anchors.targetRight != null)
			{
				_widget.rightAnchor.Set(anchors.targetRight.get_transform(), anchors.targetOffsetRelativeRight, (float)anchors.targetOffsetAbsoluteRight);
			}
			if (anchors.targetTop != null)
			{
				_widget.topAnchor.Set(anchors.targetTop.get_transform(), anchors.targetOffsetRelativeTop, (float)anchors.targetOffsetAbsoluteTop);
			}
			if (anchors.targetBottom != null)
			{
				_widget.bottomAnchor.Set(anchors.targetBottom.get_transform(), anchors.targetOffsetRelativeBottom, (float)anchors.targetOffsetAbsoluteBottom);
			}
			_widget.ResetAnchors();
			_widget.UpdateAnchors();
			Transform val = this.get_gameObject().get_transform();
			while (val != null)
			{
				UITable component = val.get_gameObject().GetComponent<UITable>();
				if (component != null)
				{
					component.Reposition();
				}
				UIPanel component2 = val.get_gameObject().GetComponent<UIPanel>();
				if (component2 != null)
				{
					break;
				}
				val = val.get_parent();
			}
		}
	}
}
