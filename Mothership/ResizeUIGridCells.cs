using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class ResizeUIGridCells : MonoBehaviour
	{
		public UIWidget uiWidget;

		public UIGrid uiGrid;

		public UISprite[] cells;

		public ResizeUIGridCells()
			: this()
		{
		}

		private unsafe void Start()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			UIWidget obj = uiWidget;
			obj.onChange = Delegate.Combine((Delegate)obj.onChange, (Delegate)new OnDimensionsChanged((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UICamera.onScreenResize = Delegate.Combine((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnEnable()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			UICamera.onScreenResize = Delegate.Combine((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnDisable()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			UIWidget obj = uiWidget;
			obj.onChange = Delegate.Remove((Delegate)obj.onChange, (Delegate)new OnDimensionsChanged((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			UICamera.onScreenResize = Delegate.Remove((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnInitialised()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			UIWidget obj = uiWidget;
			obj.onChange = Delegate.Remove((Delegate)obj.onChange, (Delegate)new OnDimensionsChanged((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			ResizeCells();
		}

		private void ResizeCells()
		{
			float num = uiWidget.get_width() / cells.Length;
			uiGrid.cellWidth = num;
			uiGrid.Reposition();
			for (int i = 0; i < cells.Length; i++)
			{
				UISprite val = cells[i];
				val.set_width(Mathf.CeilToInt(num));
			}
		}
	}
}
