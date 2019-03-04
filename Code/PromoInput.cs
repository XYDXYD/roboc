using System;

internal class PromoInput : UIInput
{
	public PromoInput()
		: this()
	{
	}

	private unsafe void Start()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		base.onValidate = Delegate.Combine((Delegate)base.onValidate, (Delegate)new OnValidate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private char ValidateIt(string text, int pos, char ch)
	{
		if (ch >= 'A' && ch <= 'Z')
		{
			return ch;
		}
		if (ch >= 'a' && ch <= 'z')
		{
			return ch;
		}
		if (ch >= '0' && ch <= '9')
		{
			return ch;
		}
		if (ch == '-')
		{
			return ch;
		}
		return '\0';
	}
}
