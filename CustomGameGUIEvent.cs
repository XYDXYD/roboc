internal struct CustomGameGUIEvent
{
	public enum Type
	{
		RefreshAndUpdateGameModeChoicesList,
		RefreshAndUpdateMapChoicesList,
		UserSetGameModeChoice,
		UserSetMapChoice,
		LeaderSetGameModeChoice,
		LeaderSetMapChoice,
		LeaderSet
	}

	private Type _type;

	private object _data;

	public Type type => _type;

	public string Data => _data.ToString();

	public CustomGameGUIEvent(Type aType, object aData = null)
	{
		_type = aType;
		_data = aData;
	}
}
