using Svelto.ECS;
using System;
using UnityEngine;
using Utility;

namespace EnginesGUI
{
	[RequireComponent(typeof(ScreenConfigPresetsComponentImplementor))]
	public class PanelSizeComponentImplementor : MonoBehaviour, IPanelSizeComponent
	{
		private DispatchOnSet<Vector2> _sizeChangeDispatcher;

		private UIPanel _panel;

		public int PanelID => _panel.GetInstanceID();

		public DispatchOnSet<Vector2> PanelSizeChanged => _sizeChangeDispatcher;

		public PanelSizeComponentImplementor()
			: this()
		{
		}

		public void Awake()
		{
			_panel = this.GetComponent<UIPanel>();
			_sizeChangeDispatcher = new DispatchOnSet<Vector2>(PanelID);
			UIPanel panel = _panel;
			panel.OnPanelResized = (Action<Vector2>)Delegate.Combine(panel.OnPanelResized, new Action<Vector2>(HandlePanelResized));
		}

		private void HandlePanelResized(Vector2 newSize)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (_sizeChangeDispatcher != null)
			{
				_sizeChangeDispatcher.set_value(newSize);
			}
		}

		public void Refresh()
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			float width = _panel.get_width();
			float height = _panel.get_height();
			Console.Log("Panel refresh triggered with width, height: " + width + "," + height);
			HandlePanelResized(new Vector2(width, height));
		}
	}
}
