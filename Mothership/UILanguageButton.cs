using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class UILanguageButton : MonoBehaviour
	{
		public UILabel label;

		public UISprite flagSprite;

		[NonSerialized]
		public string language;

		public Transform listenerParent;

		public UILanguageButton()
			: this()
		{
		}

		public void OnClick()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			new SignalChain(listenerParent).Broadcast<string>(language);
		}
	}
}
