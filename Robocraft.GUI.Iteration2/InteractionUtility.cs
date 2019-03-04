using Svelto.Tasks;
using System.Collections;
using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal static class InteractionUtility
	{
		public static bool IsParentOf(Transform root, Transform t)
		{
			while (t.get_parent() != null)
			{
				if (t.get_parent() == root)
				{
					return true;
				}
				t = t.get_parent();
			}
			return false;
		}

		public static void HideWhenClickOutside(Transform widgetToHide, Transform outsideOf = null)
		{
			if (outsideOf == null)
			{
				outsideOf = widgetToHide;
			}
			TaskRunner.get_Instance().Run(HideWhenClickOutside_Internal(widgetToHide, outsideOf));
		}

		private static IEnumerator HideWhenClickOutside_Internal(Transform widget, Transform widgetRoot)
		{
			while (true)
			{
				if (!(widget != null) || !widget.get_gameObject().get_activeInHierarchy())
				{
					yield break;
				}
				if (Input.GetMouseButtonDown(0))
				{
					GameObject hoveredObject = UICamera.get_hoveredObject();
					if (hoveredObject == null || !IsParentOf(widgetRoot, hoveredObject.get_transform()))
					{
						break;
					}
				}
				yield return null;
			}
			widget.get_gameObject().SetActive(false);
		}
	}
}
