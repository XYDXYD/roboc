using Mothership;
using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIInput))]
	[RequireComponent(typeof(BoxCollider))]
	public class UITextEntryAdapter : MonoBehaviour
	{
		private UIInput _inputArea;

		public Action<string> OnTextSubmitted;

		public Action<string> OnTextChanged;

		public Action OnInputGetFocus;

		public Action OnInputLoseFocus;

		private string _startingText;

		public UITextEntryAdapter()
			: this()
		{
		}

		public unsafe void Setup()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			_inputArea = this.GetComponent<UIInput>();
			try
			{
				_startingText = StringTableBase<StringTable>.Instance.GetString(this.get_gameObject().GetComponent<UILocalizeUIInputStartingValue>().stringKey);
				if (_startingText == null || _startingText == string.Empty)
				{
					_startingText = _inputArea.get_value();
				}
			}
			catch (Exception)
			{
				_startingText = _inputArea.get_value();
			}
			_inputArea.onReturnKey = 1;
			EventDelegate.Add(_inputArea.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			EventDelegate item = new EventDelegate(this, "OnReturnKeyPressed");
			_inputArea.onSubmit.Add(item);
			if (_inputArea is UIInputWithFocusEvents)
			{
				UIInputWithFocusEvents uIInputWithFocusEvents = _inputArea as UIInputWithFocusEvents;
				uIInputWithFocusEvents.OnInputGetFocus += HandleOnInputGetFocus;
				uIInputWithFocusEvents.OnInputLoseFocus += HandleOnInputLoseFocus;
			}
		}

		private void HandleOnInputGetFocus()
		{
			OnInputGetFocus();
		}

		private void HandleOnInputLoseFocus()
		{
			OnInputLoseFocus();
			if (_inputArea.get_value() == string.Empty)
			{
				_inputArea.set_value(_startingText);
			}
		}

		public void Clear()
		{
			_inputArea.set_value(string.Empty);
		}

		public void OnReturnKeyPressed()
		{
			OnTextSubmitted(_inputArea.get_value());
		}

		private void OnTextChange()
		{
			if (_inputArea.get_isSelected())
			{
				OnTextChanged(_inputArea.get_value());
			}
		}

		internal void ChangeTextEntryFocus(bool focus)
		{
			_inputArea.set_isSelected(focus);
		}

		public void SetTextEntryStartValue(string startValue)
		{
			_inputArea.set_value(startValue);
		}
	}
}
