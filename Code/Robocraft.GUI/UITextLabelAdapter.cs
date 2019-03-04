using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UILabel))]
	public class UITextLabelAdapter : MonoBehaviour
	{
		private UILabel _textLabel;

		public UITextLabelAdapter()
			: this()
		{
		}

		public void Setup()
		{
			_textLabel = this.GetComponent<UILabel>();
			_textLabel.set_text(string.Empty);
		}

		public void SetTextLabel(string textToSet)
		{
			_textLabel.set_text(textToSet);
		}

		public void Clear()
		{
			_textLabel.set_text(string.Empty);
		}
	}
}
