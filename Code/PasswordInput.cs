using UnityEngine;

internal class PasswordInput : UIInput
{
	public PasswordInput()
		: this()
	{
	}

	public override bool ProcessEvent(Event ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		if (((int)ev.get_keyCode() == 99 || (int)ev.get_keyCode() == 120) && (int)base.inputType == 2)
		{
			return false;
		}
		return this.ProcessEvent(ev);
	}
}
