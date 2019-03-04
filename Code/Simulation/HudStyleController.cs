using Svelto.DataStructures;

namespace Simulation
{
	internal class HudStyleController : IHudStyleController
	{
		private FasterList<IHudElement> _battleHudViews = new FasterList<IHudElement>();

		private HudStyle _battleHudStyle;

		public void AddHud(IHudElement view)
		{
			_battleHudViews.Add(view);
			view.SetStyle(_battleHudStyle);
		}

		public void RemoveHud(IHudElement view)
		{
			_battleHudViews.UnorderedRemove(view);
		}

		public void SetStyle(HudStyle style)
		{
			_battleHudStyle = style;
			for (int i = 0; i < _battleHudViews.get_Count(); i++)
			{
				_battleHudViews.get_Item(i).SetStyle(_battleHudStyle);
			}
		}
	}
}
