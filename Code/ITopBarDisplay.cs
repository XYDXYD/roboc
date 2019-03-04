using Mothership;

internal interface ITopBarDisplay
{
	void SetTopBarBuildMode(TopBarBuildMode topBarBuildMode);

	void SetTopBar(TopBar topbar);

	void SetDisplayStyle(TopBarStyle topBarStyle);

	void AddSelfToScreensList();

	TopBar GetTopBar();
}
