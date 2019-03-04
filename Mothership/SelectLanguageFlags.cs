using Login;
using Robocraft.GUI.Iteration2;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class SelectLanguageFlags : MonoBehaviour, IChainListener
	{
		public UIGrid buttonContainer;

		public UILanguageButton languageButton;

		public UILabel selectedLabel;

		public UISprite selectedSprite;

		public UIScrollView scrollView;

		public SelectLanguageFlags()
			: this()
		{
		}

		public unsafe void Start()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			InitLanguages();
			UpdateSelectedLabel();
			Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnDestroy()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnDisable()
		{
			SetListVisible(visible: false);
		}

		private void OnLanguageChanged()
		{
			UpdateSelectedLabel();
		}

		private void SetListVisible(bool visible)
		{
			buttonContainer.get_gameObject().SetActive(visible);
			if (visible)
			{
				InteractionUtility.HideWhenClickOutside(buttonContainer.get_transform(), this.get_transform());
				ResetScrollBar();
			}
		}

		public void ToggleLanguageList()
		{
			SetListVisible(!buttonContainer.get_gameObject().get_activeSelf());
		}

		void IChainListener.Listen(object message)
		{
			if (message.GetType() == typeof(SplashLoginGUIMessage))
			{
				SplashLoginGUIMessage splashLoginGUIMessage = message as SplashLoginGUIMessage;
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ShowSimpleLanguageSelector)
				{
					this.get_gameObject().SetActive(true);
				}
			}
			else
			{
				StringTableBase<StringTable>.Instance.SetLanguage(message.ToString());
				UpdateSelectedLabel();
				ToggleLanguageList();
			}
		}

		private void InitLanguages()
		{
			Dictionary<string, LanguageData> activeLanguages = StringTableBase<StringTable>.Instance.activeLanguages;
			foreach (LanguageData value in activeLanguages.Values)
			{
				string @string = StringTableBase<StringTable>.Instance.GetString(value.languageStrKey);
				GameObject val = NGUITools.AddChild(buttonContainer.get_gameObject(), languageButton.get_gameObject());
				val.get_gameObject().SetActive(true);
				UILanguageButton componentInChildren = val.GetComponentInChildren<UILanguageButton>(true);
				componentInChildren.language = @string;
				componentInChildren.label.set_text(@string);
				componentInChildren.flagSprite.set_spriteName(value.spriteName);
				val.set_name(@string);
			}
			buttonContainer.set_repositionNow(true);
			buttonContainer.get_gameObject().SetActive(false);
		}

		private void UpdateSelectedLabel()
		{
			selectedLabel.set_text(StringTableBase<StringTable>.Instance.currentLanguageString);
			selectedSprite.set_spriteName(StringTableBase<StringTable>.Instance.currentLanguageSprite);
		}

		private void ResetScrollBar()
		{
			if (scrollView != null)
			{
				scrollView.verticalScrollBar.set_value(0f);
			}
		}
	}
}
