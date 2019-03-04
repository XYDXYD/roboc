using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UITextLabelAdapter))]
	public class GenericTextLabelComponentView : GenericComponentViewBase
	{
		private UITextLabelAdapter _textLabelAdapter;

		public override void Setup()
		{
			base.Setup();
			_textLabelAdapter = this.GetComponent<UITextLabelAdapter>();
			_textLabelAdapter.Setup();
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void SetTextLabel(string textToSet)
		{
			_textLabelAdapter.SetTextLabel(textToSet);
		}
	}
}
