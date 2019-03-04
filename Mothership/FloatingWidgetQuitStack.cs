using System.Collections.Generic;

namespace Mothership
{
	internal class FloatingWidgetQuitStack
	{
		private readonly List<IFloatingWidget> _widgets = new List<IFloatingWidget>();

		public void Add(IFloatingWidget w)
		{
			_widgets.Add(w);
		}

		public void Remove(IFloatingWidget w)
		{
			_widgets.Remove(w);
		}

		public bool HandleQuitPressed()
		{
			if (_widgets.Count == 0)
			{
				return false;
			}
			IFloatingWidget floatingWidget = _widgets[_widgets.Count - 1];
			floatingWidget.HandleQuitPressed();
			return true;
		}
	}
}
