using Mothership;
using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UITextEntryAdapter))]
	public class GenericTextEntryComponentView : GenericComponentViewBase
	{
		private UITextEntryAdapter _textEntry;

		public override void Setup()
		{
			base.Setup();
			_textEntry = this.GetComponent<UITextEntryAdapter>();
			_textEntry.Setup();
			UITextEntryAdapter textEntry = _textEntry;
			textEntry.OnTextSubmitted = (Action<string>)Delegate.Combine(textEntry.OnTextSubmitted, new Action<string>(OnTextSubmitted));
			UITextEntryAdapter textEntry2 = _textEntry;
			textEntry2.OnTextChanged = (Action<string>)Delegate.Combine(textEntry2.OnTextChanged, new Action<string>(OnTextChanged));
			UITextEntryAdapter textEntry3 = _textEntry;
			textEntry3.OnInputGetFocus = (Action)Delegate.Combine(textEntry3.OnInputGetFocus, new Action(OnInputGetFocus));
			UITextEntryAdapter textEntry4 = _textEntry;
			textEntry4.OnInputLoseFocus = (Action)Delegate.Combine(textEntry4.OnInputLoseFocus, new Action(OnInputLoseFocus));
		}

		public override void Listen(object message)
		{
			base.Listen(message);
			if (message is SocialMessage)
			{
				SocialMessage socialMessage = message as SocialMessage;
				(_controller as GenericTextEntryComponent).HandleSocialMessage(socialMessage);
			}
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		internal void OnTextSubmitted(string textField)
		{
			(_controller as GenericTextEntryComponent).HandleTextSubmitted(textField);
		}

		private void OnTextChanged(string textField)
		{
			(_controller as GenericTextEntryComponent).HandleTextChanged(textField);
		}

		private void OnInputGetFocus()
		{
			(_controller as GenericTextEntryComponent).HandleInputGetFocus();
		}

		private void OnInputLoseFocus()
		{
			(_controller as GenericTextEntryComponent).HandleInputLoseFocus();
		}

		public void ChangeTextEntryFocus(bool focus)
		{
			_textEntry.ChangeTextEntryFocus(focus);
		}

		public void SetTextEntryStartValue(string startValue)
		{
			_textEntry.SetTextEntryStartValue(startValue);
		}
	}
}
