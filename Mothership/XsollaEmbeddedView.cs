using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mothership
{
	internal class XsollaEmbeddedView : MonoBehaviour
	{
		public Text textlabel;

		public string stringKey;

		public Action CloseButtonClicked;

		public RawImage backgroundImage;

		public Color transparentBackgroundColour;

		public Color solidBackgroundColour;

		public XsollaEmbeddedView()
			: this()
		{
		}

		private void Start()
		{
			textlabel.set_text(StringTableBase<StringTable>.Instance.GetString(stringKey));
		}

		public void CloseClicked()
		{
			CloseButtonClicked();
		}

		public void SetBackgroundOpaque()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			backgroundImage.set_color(solidBackgroundColour);
		}

		public void SetBackgroundTransparent()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			backgroundImage.set_color(transparentBackgroundColour);
		}
	}
}
