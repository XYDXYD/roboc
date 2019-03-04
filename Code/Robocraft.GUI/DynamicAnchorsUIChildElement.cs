using UnityEngine;

namespace Robocraft.GUI
{
	public class DynamicAnchorsUIChildElement : MonoBehaviour, IAnchorableUIElement
	{
		public DynamicAnchorsUIChildElement()
			: this()
		{
		}

		public void AnchorThisElementUnder(IAnchorUISource other)
		{
			UIRect anchorSource = other.GetAnchorSource();
			AnchorThisElementUnder(anchorSource);
		}

		public void AnchorThisElementUnder(UIRect otherWidget)
		{
			UIRect component = this.GetComponent<UIRect>();
			component.SetAnchor(otherWidget.get_transform());
			component.leftAnchor.Set(0f, 0f);
			component.rightAnchor.Set(1f, 0f);
			component.topAnchor.Set(1f, 0f);
			component.bottomAnchor.Set(0f, 0f);
		}

		public void ReparentOnly(Transform other)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_parent(other);
			this.get_transform().set_localScale(Vector3.get_one());
		}
	}
}
