using System;

namespace Login
{
	internal sealed class SplashLoginErrorDialogConfiguration
	{
		public enum ButtonType
		{
			OK,
			Cancel,
			Retry,
			Quit
		}

		public ButtonType[] buttonTypes = new ButtonType[3];

		public Action<SplashLoginGUIMessageType> OnActionCallback;

		public string header
		{
			get;
			private set;
		}

		public string body
		{
			get;
			private set;
		}

		public int numButtons
		{
			get;
			private set;
		}

		public SplashLoginErrorDialogConfiguration(string headerText, string bodyText, ButtonType buttonType1)
		{
			header = headerText;
			body = bodyText;
			numButtons = 1;
			buttonTypes[0] = buttonType1;
		}

		public SplashLoginErrorDialogConfiguration(string headerText, string bodyText, ButtonType buttonType1, ButtonType buttonType2)
			: this(headerText, bodyText, buttonType1)
		{
			buttonTypes[1] = buttonType2;
		}

		public SplashLoginErrorDialogConfiguration(string headerText, string bodyText, ButtonType buttonType1, Action<SplashLoginGUIMessageType> callback)
		{
			header = headerText;
			body = bodyText;
			numButtons = 1;
			buttonTypes[0] = buttonType1;
			OnActionCallback = callback;
		}

		public SplashLoginErrorDialogConfiguration(string headerText, string bodyText, ButtonType buttonType1, ButtonType buttonType2, Action<SplashLoginGUIMessageType> callback)
			: this(headerText, bodyText, buttonType1)
		{
			buttonTypes[1] = buttonType2;
			OnActionCallback = callback;
		}
	}
}
