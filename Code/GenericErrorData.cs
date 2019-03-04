using System;

internal sealed class GenericErrorData
{
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

	public string okButtonText
	{
		get;
		private set;
	}

	public string cancelButtonText
	{
		get;
		private set;
	}

	public Action onOkClicked
	{
		get;
		private set;
	}

	public Action onCancelClicked
	{
		get;
		private set;
	}

	public GenericErrorData()
	{
	}

	public GenericErrorData(string headerText, string bodyText, string okText, string cancelText, Action okClicked, Action cancelClicked)
		: this(headerText, bodyText, okText, okClicked)
	{
		cancelButtonText = cancelText;
		onCancelClicked = cancelClicked;
	}

	public GenericErrorData(string headerText, string bodyText, string okText, Action okClicked)
		: this(headerText, bodyText, okText)
	{
		onOkClicked = okClicked;
	}

	public GenericErrorData(string headerText, string bodyText, string okText)
		: this(headerText, bodyText)
	{
		okButtonText = okText;
	}

	public GenericErrorData(string headerText, string bodyText)
	{
		header = headerText;
		body = bodyText;
		okButtonText = StringTableBase<StringTable>.Instance.GetString("strOK");
		cancelButtonText = null;
		onCancelClicked = null;
	}
}
