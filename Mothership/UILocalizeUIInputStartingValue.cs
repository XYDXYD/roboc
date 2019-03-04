using System;
using UnityEngine;

namespace Mothership
{
	[RequireComponent(typeof(UIInput))]
	internal class UILocalizeUIInputStartingValue : MonoBehaviour
	{
		public string stringKey;

		private UIInput _input;

		public UILocalizeUIInputStartingValue()
			: this()
		{
		}

		private unsafe void Start()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			_input = this.GetComponent<UIInput>();
			UpdateStartingValueString();
			Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnDestroy()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void UpdateStartingValueString()
		{
			_input.set_value(StringTableBase<StringTable>.Instance.GetString(stringKey));
		}
	}
}
