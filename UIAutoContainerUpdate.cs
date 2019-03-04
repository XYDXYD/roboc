using Robocraft.GUI.Iteration2;
using UnityEngine;

[ExecuteInEditMode]
public class UIAutoContainerUpdate : MonoBehaviour
{
	public UIAutoContainerUpdate()
		: this()
	{
	}

	private void OnEnable()
	{
		NotifyContainer();
	}

	private void OnDisable()
	{
		NotifyContainer();
	}

	private void OnTransformParentChanged()
	{
		NotifyContainer();
	}

	private void NotifyContainer()
	{
		Transform parent = this.get_transform().get_parent();
		if (!(parent == null))
		{
			UIWidgetContainer component = parent.GetComponent<UIWidgetContainer>();
			if (component != null)
			{
				LayoutUtility.ScheduleReposition(component);
			}
		}
	}
}
