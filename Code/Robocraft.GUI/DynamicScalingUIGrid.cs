using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[AddComponentMenu("NGUI/Interaction/DynamicScalingUIGrid")]
	[RequireComponent(typeof(UIGrid))]
	public class DynamicScalingUIGrid : MonoBehaviour
	{
		public UIWidget target;

		public float desiredNumElementsVertical = 3f;

		public bool WidthShouldMatchHeight;

		private UIGrid _uiGrid;

		public DynamicScalingUIGrid()
			: this()
		{
		}

		public unsafe void Start()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			_uiGrid = this.GetComponent<UIGrid>();
			this.set_enabled(false);
			UICamera.onScreenResize = Delegate.Combine((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnDestroy()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			UICamera.onScreenResize = Delegate.Remove((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Update()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localPosition = _uiGrid.get_transform().get_localPosition();
			localPosition.Set(0f, 0f, 0f);
			_uiGrid.cellHeight = (float)target.get_height() / desiredNumElementsVertical;
			if (WidthShouldMatchHeight)
			{
				_uiGrid.cellWidth = _uiGrid.cellHeight;
			}
			_uiGrid.Reposition();
			IDynamicGridResizingChildElement[] componentsInChildren = this.GetComponentsInChildren<IDynamicGridResizingChildElement>();
			IDynamicGridResizingChildElement[] array = componentsInChildren;
			foreach (IDynamicGridResizingChildElement dynamicGridResizingChildElement in array)
			{
				dynamicGridResizingChildElement.ResizeAccordingToGridRequirements(_uiGrid.cellWidth, _uiGrid.cellHeight);
			}
			this.set_enabled(false);
		}

		public void RequiresRecalculation()
		{
			this.set_enabled(true);
		}
	}
}
