internal static class PartyAndSocialGUIEditModeCursorSwitch
{
	internal static void PopCursorModeIfClickNotInSocialOrParty(ICursorMode cursorMode)
	{
		if (WorldSwitching.IsInBuildMode())
		{
			cursorMode.PopFreeMode();
		}
	}
}
